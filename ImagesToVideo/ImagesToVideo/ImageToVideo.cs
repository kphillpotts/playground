using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using UIKit;

namespace ImagesToVideo
{
	public class ImageToVideo
	{
		public async Task<bool> WriteImagesToVideo(string[] images, string outputVideoPath)
		{
		    try
		    {
		        AVAssetWriter assetWriter;
		        AVAssetWriterInput writerInput;
		        AVAssetWriterInputPixelBufferAdaptor adaptor;

                var videoUrl = NSUrl.FromFilename(outputVideoPath);

                //TODO: Get appsed in Size
                var frameSize = new CGSize(640, 480);

                // Initialize AVAssetWriter
                NSError error;
                assetWriter = new AVAssetWriter(videoUrl, AVFileType.QuickTimeMovie, out error);
                if (error != null)
                {
                    Console.WriteLine(error.LocalizedDescription);
                    return false;
                }

                // Initialize AssetWriterInput
                var videoSettings = NSDictionary.FromObjectsAndKeys(
                new NSObject[] { AVVideo.CodecH264, new NSNumber(frameSize.Width), new NSNumber(frameSize.Height) },
                new NSObject[] { AVVideo.CodecKey, AVVideo.WidthKey, AVVideo.HeightKey }
                        );

		        AVVideoSettingsCompressed compressedVideoSettings = new AVVideoSettingsCompressed(videoSettings);
                writerInput = new AVAssetWriterInput(AVMediaType.Video, compressedVideoSettings);

                // create pixel buffer adaptor
                NSDictionary sourcePixelBUfferAttributesDictionary = NSDictionary.FromObjectAndKey(
		            NSNumber.FromObject(CVPixelFormatType.CV32ARGB), CVPixelBuffer.PixelFormatTypeKey);

                adaptor = new AVAssetWriterInputPixelBufferAdaptor(writerInput, sourcePixelBUfferAttributesDictionary);

                assetWriter.AddInput(writerInput);
                writerInput.ExpectsMediaDataInRealTime = true; 
                
                assetWriter.StartWriting();
		        assetWriter.StartSessionAtSourceTime(CMTime.Zero);

                // TODO: Should not get Images at this point, should get inside WriteImages.
                // otherwise we are instantiating a bunch of stuff we don't need to.
		        List<UIImage> uiImages = new List<UIImage>();
		        foreach (var imageName in images)
		        {
		            uiImages.Add(UIImage.FromFile(imageName));
		        }

		        var result = WriteImages(writerInput, assetWriter, adaptor, uiImages.ToArray());

                writerInput.MarkAsFinished();
		        await assetWriter.FinishWritingAsync();

                assetWriter.Dispose();
                writerInput.Dispose();

		        return true;
		    }
		    catch (Exception ex)

		    {
		        return false;
		    }

		}

		private CVPixelBuffer PixelBufferFromCGImage(CGImage image, AVAssetWriterInputPixelBufferAdaptor adaptor)
		{
		    CVPixelBuffer returnBuffer = null;
		    try
		    {
			    CVReturn err = new CVReturn();
                if (returnBuffer == null)
			    {
                    returnBuffer = adaptor.PixelBufferPool.CreatePixelBuffer();// (mCVPixelBufferPoolAllocationSettings, out err);
			    }

                if (returnBuffer != null)
			    {
                    returnBuffer.Lock(0);
                    IntPtr pxdata = returnBuffer.BaseAddress;

				    if (pxdata != IntPtr.Zero)
				    {
					    using (var rgbColorSpace = CGColorSpace.CreateDeviceRGB())
					    {
					        try
					        {
						        using (CGBitmapContext context = new CGBitmapContext(pxdata, image.Width, image.Height, 8, 4 * image.Width, rgbColorSpace, CGImageAlphaInfo.NoneSkipFirst))
						        {
							        if (context != null)
							        {
								        context.DrawImage(new RectangleF(0, 0, image.Width, image.Height), image);
							        }

							        context.Dispose();
						        }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                        }
				    }
                    returnBuffer.Unlock(0);
			    }

                return returnBuffer;
                }
            catch (Exception ex)
            {
                Console.WriteLine("Exceptino! " + ex.StackTrace);
                return null;
            }

		}


		private bool WriteImages(AVAssetWriterInput writerInput, AVAssetWriter assetWriter, AVAssetWriterInputPixelBufferAdaptor adaptor, UIImage[] images)
		{
			int i = 0;
			while (true)
			{
				if (writerInput.ReadyForMoreMediaData)
				{
					CMTime frameTime = new CMTime(0,20);
					CMTime lastTime = new CMTime(i, 20);
					CMTime presentTime = CMTime.Add(lastTime, frameTime);

				    if (i >= images.Count())
				    {
				        return true;
				    }

				    var img = images[i].CGImage;
				    var buffer = PixelBufferFromCGImage(img, adaptor);

				    adaptor.AppendPixelBufferWithPresentationTime(buffer, presentTime);

				    i++;
				}
				
			}
			return false;
		}


	}
}