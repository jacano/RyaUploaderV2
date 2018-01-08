using System.IO;
using RyaUploaderV2.Extensions;
using RyaUploaderV2.Models;
using RyaUploaderV2.Services;
using Stylet;
using StyletIoC;
using RyaUploaderV2.ViewModels;

namespace RyaUploaderV2
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
            builder.Bind<IPathService>().To<PathService>();
            builder.Bind<IShareCodeService>().To<ShareCodeService>();
            builder.Bind<IBoilerProcessService>().To<BoilerProcessService>();
            builder.Bind<IUploadService>().To<UploadService>().InSingletonScope();
            builder.Bind<IFileReadingService>().To<FileReadingReadingService>();

            // Singleton for the client to get an infinite running timer
            builder.Bind<BoilerClient>().ToSelf().InSingletonScope();
            builder.Bind<TrayIconViewModel>().ToSelf().InSingletonScope();
        }

        protected override void OnStart()
        {
            base.OnStart();

            var folder = Path.Combine(Path.GetTempPath(), "RyaUploader");
            Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, "boiler.exe");
            File.WriteAllBytes(path, Properties.Resources.Boiler);
            path = Path.Combine(folder, "libEGL.dll");
            File.WriteAllBytes(path, Properties.Resources.libEGL);
            path = Path.Combine(folder, "libGLESv2.dll");
            File.WriteAllBytes(path, Properties.Resources.libGLESv2);
            path = Path.Combine(folder, "msvcp120.dll");
            File.WriteAllBytes(path, Properties.Resources.msvcp120);
            path = Path.Combine(folder, "msvcr120.dll");
            File.WriteAllBytes(path, Properties.Resources.msvcr120);
            path = Path.Combine(folder, "protobuf-net.dll");
            File.WriteAllBytes(path, Properties.Resources.protobuf_net);
            path = Path.Combine(folder, "steam_api.dll");
            File.WriteAllBytes(path, Properties.Resources.steam_api);
            path = Path.Combine(folder, "steam_appid.txt");
            File.WriteAllBytes(path, Properties.Resources.steam_appid);
        }

        protected override void DisplayRootView(object rootViewModel)
        {
            base.DisplayRootView(rootViewModel);
            
            GetActiveWindow().EnableBlur();
        }
    }
}
