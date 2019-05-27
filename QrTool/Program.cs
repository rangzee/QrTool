using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using ZXing;
using ZXing.Common;

namespace QrTool
{
    // Use ILMerge to generate single file QR.exe:
    //  - ILMerge /ndebug /target:winexe /out:QR.exe /log QrTool.exe zxing.dll /targetplatform:v4
    //  - ILMerge QrTool.exe zxing.dll /out:QR.exe /targetplatform:v4
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                try
                {
                    var filename = args[0];
                    var bitmap = new Bitmap(filename);

                    BarcodeReader reader = new BarcodeReader();
                    reader.AutoRotate = true;
                    reader.TryInverted = true;
                    reader.Options.TryHarder = true;
                    reader.Options.CharacterSet = "UTF-8";
                    Result result = reader.Decode(bitmap);
                    Console.WriteLine(result.Text);
                }
                catch
                {
                    Console.WriteLine("invalid qr image file.");
                }
            }
            else if (args.Length == 3)
            {
                var width = args[0]; // 二维码宽度
                var height = args[1]; // 二维码高度
                var content = args[2]; // 编码内容
                try
                {
                    BarcodeWriter barCodeWriter = new BarcodeWriter();
                    barCodeWriter.Format = BarcodeFormat.QR_CODE;
                    barCodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
                    barCodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
                    barCodeWriter.Options.Height = int.TryParse(height, out int h) ? h : 512;
                    barCodeWriter.Options.Width = int.TryParse(width, out int w) ? w : 512;
                    barCodeWriter.Options.Margin = 0;

                    BitMatrix bm = barCodeWriter.Encode(content);
                    var bitmap = barCodeWriter.Write(bm);

                    bitmap.Save($"QR_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png", System.Drawing.Imaging.ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                var execute = System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase).Replace(".exe", "");
                Console.WriteLine($"Usage:\n\t{execute} width height \"text to code\"");
                Console.WriteLine($"\t{execute} \"file to decode\"");
            }
        }

    }
}
