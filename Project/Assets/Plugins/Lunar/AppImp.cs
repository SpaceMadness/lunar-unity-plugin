using LunarPlugin;

namespace LunarPluginInternal
{
    interface AppImp : IUpdatable, IDestroyable
    {
        void Start();
        void Stop();

        bool ExecCommand(string commandLine, bool manual);
    }
}

