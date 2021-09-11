using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using CovertWorker.Extensions;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace CovertWorker
{
    public class Worker : BackgroundService
    {
        readonly ServiceConfig config;

        public Worker(ServiceConfig config)
        {
            this.config = config;
        }

        private async Task Execute(ServiceTargetOption target)
        {
            var logPath = Path.Combine(target.Path, "worker.log");

            #region Prepare Paths
            var path = target.Path;

            var backup = target.Backup;
            backup = Path.Combine(path, backup);
            Directory.CreateDirectory(backup);

            var converterPath = target.Converter.Path;
            var converterExe = target.Converter.Exe;
            var converterArgs = target.Converter.Args;
            converterPath = Path.Combine(path, converterPath);
            converterExe = Path.Combine(converterPath, converterExe);

            var inPattern = new Regex(target.In);
            var outPattern = target.Out;
            var files = Directory.GetFiles(path)
                .Where(x => inPattern.IsMatch(x));
            #endregion

            #region Convert
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var ext = Path.GetExtension(file);

                var backupFile = Path.Combine(backup, name + ext);
                var converterFile = Path.Combine(converterPath, name + ext);

                var outFile = outPattern;
                outFile = outFile.Format(
                    new Dictionary<string, object>
                    {
                        ["name"] = name,
                        ["ext"] = ext
                    });

                var outputResultFile = Path.Combine(path, outFile);
                var outputConverterFile = Path.Combine(converterPath, outFile);

                var args = converterArgs.Format(
                    new Dictionary<string, object>
                    {
                        ["name"] = name,
                        ["ext"] = ext
                    });

                File.Copy(file, converterFile, true);
                File.Move(file, backupFile, true);

                var startInfo = new ProcessStartInfo()
                {
                    Arguments = args,
                    WorkingDirectory = converterPath,
                    FileName = converterExe,
                    CreateNoWindow = false
                };

                var process = Process.Start(startInfo);

                await process.WaitForExitAsync();

                File.Delete(converterFile);
                File.Move(outputConverterFile, outputResultFile);

                await File.AppendAllTextAsync(logPath, 
                    $"Processed:\n\t{name}{ext}\t\t=>\t\t{outFile}" + '\n');
            }
            #endregion
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var target in config.Targets)
                {
                    try
                    {
                        await Execute(target);
                    }
                    catch (Exception e)
                    {
                        var errorPath = Path.Combine(target.Path, "error.log");

                        await File.AppendAllTextAsync(errorPath, e.Message + '\n');
                    }
                }

                Thread.Sleep(10000);
            }
        }
    }
}
