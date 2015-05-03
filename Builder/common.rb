require 'pathname'

module Lunar

  ############################################################

  class CopingEntry
    attr_reader :src, :dest, :names

    def initialize(dest, src, *names)
      @dest = dest
      @src = src
      @names = names
    end
  end

  ############################################################

  # check condition and raise exception
  def fail_script(message)
    raise "Build failed! #{message}"
  end

  ############################################################

  # check condition and raise exception
  def fail_script_unless(condition, message)
    fail_script message unless condition
  end

  ############################################################

  # check condition and raise exception
  def fail_script_if(condition, message)
    fail_script message if condition
  end

  ############################################################

  # check if file exists and raise exception
  def fail_script_unless_file_exists(path, message = nil)
    fail_script_unless File.directory?(path) || File.exists?(path), (message.nil? ? "File doesn't exist: '#{path}'" : message)
  end

  ############################################################

  # checks if path exists and returns it
  def resolve_path(path)
    fail_script_unless_file_exists path
    return path
  end

  ############################################################

  def copy_file(file, dest)
    if File.directory? file
      FileUtils.cp_r file, dest
    else
      FileUtils.cp file, dest
    end
  end

  ############################################################

  def delete_file(path)
    if File.directory? path
      FileUtils.rm_rf path
    else
      FileUtils.rm_f path
    end
  end

  ############################################################

  def delete_asset(path)
    file_meta = "#{path}.meta"
    delete_file path
    delete_file file_meta
  end

  ############################################################

  def delete_all(mask)
    Dir[mask].each {|file| delete_file file }
  end

  ############################################################

  def delete_all_assets(mask)
    Dir[mask].each {|file| delete_asset file }
  end

  ############################################################

  def list_assets(mask)
    assets = []

    Dir[mask].each do |file|
      ext = File.extname file
      if ext == '.meta'
        next
      end

      assets << file
    end

    return assets
  end

  ############################################################

  def cleanup_project(existing_files, builder_files)

    remaining_files = []
    existing_files.each {|file|
      if (builder_files.include? file)
        remaining_files.push file
        next
      end

      if (File.directory?(file))
        found = false
        builder_files.each do |builder_file|
          if builder_file.start_with? file
            remaining_files.push file
            found = true
            break
          end
        end

        next if found
      end

      delete_file file
    }

    return remaining_files
  end

  ############################################################

  def exec_shell(command, error_message, &error_handler)
    puts "Running command: #{command}"
    result = `#{command}`
    unless $?.success?
      if block_given?
        error_handler.call $1, result
      else
        fail_script "#{error_message}\nShell failed: #{command}\n#{result}"
      end
    end

    return result
  end

  ############################################################

  def make_relative_path(first, second)
    first_path = Pathname.new first
    second_path = Pathname.new second

    return first_path.relative_path_from(second_path).to_s
  end

  ############################################################

  def force_windows_path_separator path
    return path.sub '/', '\\'
  end

  ############################################################

  def create_include path, link
    file_path = force_windows_path_separator(File.expand_path path)
    file_link = force_windows_path_separator link

    return %(<Compile Include="#{file_path}">\n  <Link>#{file_link}</Link>\n</Compile>)
  end

  ############################################################

  def create_includes(dir_root, files)
    includes = []

    files.each { |file|
      link_path = file.sub "#{dir_root}/", ''
      includes << create_include(file, link_path)
    }

    return includes
  end

  ############################################################

  def build_ios_library(proj_dir, proj_name, configuration, target, options = {})
    Dir.chdir(proj_dir) do

      # cleanup
      FileUtils.rm_rf('build')

      # build
      cmd = %(xcodebuild -project "#{proj_name}.xcodeproj" -configuration "#{configuration}" -target "#{target}" -sdk iphoneos clean build)
      cmd += ' RUN_CLANG_STATIC_ANALYZER=NO'
      exec_shell cmd, "Can't build ios library"

      # output
      file_lib = File.expand_path "build/#{configuration}-iphoneos/lib#{target}.a"
      fail_script_unless_file_exists file_lib

      file_dest = options[:lib_dest]
      if file_dest
        FileUtils.cp file_lib, file_dest
        return file_dest
      end

      return file_lib
    end
  end

  ############################################################

  def check_ios_library_architectures file_lib, *expected_archs
    fail_script_unless_file_exists file_lib

    output = exec_shell %(lipo -info "#{file_lib}"), "Can't get lib info"
    output =~ /Architectures in the fat file: (.*?\.a) are: (.*)/

    fail_script_unless $2 != nil, "Can't extract architectures: #{output}"

    actual_archs = $2.split ' '

    # check missing archs
    expected_archs.each { |arch|
      fail_script_unless actual_archs.include?(arch), "Missing required arch: #{arch}"
    }

    # check unwanted extra archs
    actual_archs.each { |arch|
      fail_script_unless expected_archs.include?(arch), "Unexpected extra arch: #{arch}"
    }

  end

  ############################################################

  def build_android_aar(proj_dir, module_name, options = {})
    Dir.chdir(proj_dir) do

      config = options.include?(:build_config) ? options[:build_config] : 'release'

      fail_script_unless_file_exists 'gradlew'
      exec_shell "./gradlew :#{module_name}:build", "Can't build module"

      file_lib = File.expand_path "#{module_name}/build/outputs/aar/#{module_name}-#{config}.aar"
      fail_script_unless_file_exists file_lib

      return file_lib
    end

  end

  ############################################################

  def build_android_jar(proj_dir, module_name, options = {})
    file_aar = build_android_aar proj_dir, module_name, options

    file_jar = "#{File.dirname file_aar}/#{File.basename file_aar, '.*'}.jar"
    delete_file file_jar

    exec_shell %(unzip -p "#{file_aar}" classes.jar > "#{file_jar}"), "Can't unzip classes.jar"
    fail_script_unless_file_exists file_jar

    return File.expand_path file_jar
  end

  ############################################################

  def build_mono_project(file_proj)
    fail_script_unless_file_exists file_proj

    mdtool_path = '/Applications/Unity/MonoDevelop.app/Contents/MacOS/mdtool'
    fail_script_unless_file_exists mdtool_path

    exec_shell %("#{mdtool_path}" build "#{File.expand_path file_proj}"), "Can't build project"
  end

  ############################################################

  def generate_build_mono_project proj_template, dir_out, options
    proj_name = File.basename proj_template, '.template'
    path_proj = "#{dir_out}/#{proj_name}"

    template = Lunar::TemplateFile.new proj_template
    template.process path_proj, options

    build_mono_project path_proj
  end

  ############################################################

  def resolve_unity_bin
    unity_path = '/Applications/Unity/Unity.app/Contents/MacOS/Unity'
    fail_script_unless_file_exists unity_path, "Unity installation not found: #{unity_path}"

    return unity_path
  end

  ############################################################

  def exec_unity_method(project, method)

    cmd = %(#{resolve_unity_bin} -quit -batchmode -executeMethod #{method} -projectPath "#{project}")
    exec_shell(cmd, nil) do |code, result|

      file_unity_editor_log = File.expand_path '~/Library/Logs/Unity/Editor.log'
      fail_script_unless_file_exists file_unity_editor_log

      unity_editor_log = File.read file_unity_editor_log
      fail_script "Can't execute method: #{method}\nProject: #{project}\n#{unity_editor_log}"
    end

    unity_log = File.expand_path '~/Library/Logs/Unity/Editor.log'
    fail_script_unless_file_exists unity_log

    result = File.read unity_log
    result =~ /(Exiting batchmode successfully now!)/

    fail_script_unless $1 != nil, "Unity batch failed\n#{result}"
  end

end