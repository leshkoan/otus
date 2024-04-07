using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spacebattle.PluginLoader
{
    interface IPlugin
    {
        void Load();
    }

    class PluginLoader
    {
        private readonly Queue<IPlugin> _pluginQueue = new Queue<IPlugin>();
        private readonly object _lock = new object();

        public void ScanPlugins(string folderPath)
        {
            string[] pluginFiles = Directory.GetFiles(folderPath, "*.dll");

            foreach (var pluginFile in pluginFiles)
            {
                Assembly assembly = Assembly.LoadFrom(pluginFile);

                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IPlugin).IsAssignableFrom(type))
                    {
                        IPlugin? plugin = Activator.CreateInstance(type) as IPlugin;
                        if (plugin != null) EnqueuePlugin(plugin);
                    }
                }
            }
        }

        private void EnqueuePlugin(IPlugin plugin)
        {
            lock (_lock)
            {
                _pluginQueue.Enqueue(plugin);
            }
        }

        public void LoadPlugins()
        {
            while (true)
            {
                IPlugin plugin = DequeuePlugin();

                try
                {
                    plugin.Load();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading plugin: {ex.Message}");
                    EnqueuePlugin(plugin); // Повторная попытка загрузки плагина
                }
            }
        }

        private IPlugin DequeuePlugin()
        {
            lock (_lock)
            {
                return _pluginQueue.Dequeue();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PluginLoader pluginLoader = new PluginLoader();
            string pluginsFolderPath = "C:\\plugins";

            Thread scanThread = new Thread(() => pluginLoader.ScanPlugins(pluginsFolderPath));
            Thread loadThread = new Thread(pluginLoader.LoadPlugins);

            scanThread.Start();
            loadThread.Start();

            scanThread.Join();
            loadThread.Join();
        }
    }
}
