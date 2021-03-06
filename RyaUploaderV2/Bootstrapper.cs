using System.IO;
using System.Net.Http;
using RyaUploaderV2.Extensions;
using RyaUploaderV2.Facade;
using RyaUploaderV2.Models;
using RyaUploaderV2.Services;
using RyaUploaderV2.Services.Converters;
using RyaUploaderV2.Services.FileServices;
using RyaUploaderV2.Services.SteamServices;
using Stylet;
using StyletIoC;
using RyaUploaderV2.ViewModels;
using Serilog;

namespace RyaUploaderV2
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            Log.Information("Configuring the IoC container.");

            // Add all Transient services to the IoC
            builder.Bind<IFilePaths>().To<FilePaths>();
            builder.Bind<IFileReader>().To<FileReader>();
            builder.Bind<IFileWriter>().To<FileWriter>();

            builder.Bind<IShareCodeConverter>().To<ShareCodeConverter>();
            builder.Bind<IProtobufConverter>().To<ProtobufConverter>();

            builder.Bind<IBoilerProcess>().To<BoilerProcess>();
            builder.Bind<ISteamApi>().To<SteamApi>();

            // Add all Singleton services to the IoC
            builder.Bind<IUploader>().To<Uploader>().InSingletonScope();

            // Add all other Singleton classes to the IoC
            builder.Bind<BoilerClient>().ToSelf().InSingletonScope();
            builder.Bind<TrayIconViewModel>().ToSelf().InSingletonScope();

            // Add static httpclient
            builder.Bind<IHttpClient>().ToInstance(new HttpClientFacade());

            Log.Information("Finished configuring the IoC container.");
        }

        protected override void OnStart()
        {
            base.OnStart();

            SetupLogger();

            var folder = Path.Combine(Path.GetTempPath(), "RyaUploader");
            Directory.CreateDirectory(folder);

            Log.Information("Saving boiler.exe to the temp folder.");

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

            Log.Information("Finished saving boiler.exe to the temp folder.");
        }

        private void SetupLogger()
        {
            var appDataFolder = FilePaths.GetAppDataPath();
            var logFile = Path.Combine(appDataFolder, "RyaUploader-.log");

            // Seting up serilog to save to a log file.
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        protected override void DisplayRootView(object rootViewModel)
        {
            base.DisplayRootView(rootViewModel);
            
            GetActiveWindow().EnableBlur();
        }
    }
}
