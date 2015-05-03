module Lunar
  class TemplateFile
    attr_reader :path

    def initialize(path)
      @path = path
    end

    def process(file_dest, options = {})
      text = File.read @path
      options.each do |name, value|
        text =~ /((\s*)(\${(#{name})}))/
        if $1
          lines = value.split "\n"
          text.gsub! $3, lines.join($2)
        end

      end

      File.write file_dest, text

    end
  end
end