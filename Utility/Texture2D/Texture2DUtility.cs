using UnityEngine;
using System.Collections;
using System;
using System.IO;

public static class Texture2DUtility
{
	public static Vector2 GetRescaleSize (Texture2D source, int maxWidth, int maxHeight)
	{
		Vector2 rescaleSize = Vector2.zero;
		if (source.width > maxWidth || source.height > maxHeight) {
			bool baseOnWidth = source.width > source.height;
			float rescaleRatio = baseOnWidth ? ((float)maxWidth / (float)source.width) : ((float)maxHeight / (float)source.height);
			rescaleSize.x = Mathf.RoundToInt (rescaleRatio * source.width);
			rescaleSize.y = Mathf.RoundToInt (rescaleRatio * source.height);
		} else {
			rescaleSize.x = source.width;
			rescaleSize.y = source.height;
		}

		return rescaleSize;
	}

	public static Texture2D ScaleTexture (Texture2D source, int targetWidth, int targetHeight)
	{
		if (source.width == targetWidth && source.height == targetHeight)
			return source;

		Texture2D result = new Texture2D (targetWidth, targetHeight, source.format, true);
		Color[] rpixels = result.GetPixels (0);
		float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
		float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);
		for (int px = 0; px < rpixels.Length; px++) {
			rpixels [px] = source.GetPixelBilinear (incX * ((float)px % targetWidth),
				incY * ((float)Mathf.Floor (px / targetWidth)));
		}
		result.SetPixels (rpixels, 0);
		result.Apply ();

		return result;
	}

	public static Texture2D CropTexture (Texture2D source, 
	                                    int targetWidth, 
	                                    int targetHeight, 
	                                    int xOffset = 0, 
	                                    int yOffset = 0)
	{
		Texture2D result = new Texture2D (targetWidth, targetHeight, source.format, true);

		for (int rx = 0; rx < result.width; rx++) {
			for (int ry = 0; ry < result.height; ry++) {
				int sx = rx + xOffset;
				int sy = ry + yOffset;
				if (sx >= 0 && sy >= 0) {
					Color pixel = source.GetPixel (sx, sy);
					result.SetPixel (rx, ry, pixel);
				}
			}
		}
		result.Apply ();

		return result;
	}

	public static Texture2D CopyTexture (Texture2D source)
	{
		Texture2D result = new Texture2D (source.width, source.height, source.format, true);
		result.SetPixels (source.GetPixels (), 0);
		result.Apply ();
		return result;
	}

	public static void OverlayImage (Texture2D image, Texture2D overlay, Vector2 offset)
	{
		for (int x = 0; x < overlay.width; x++) {
			int shiftedX = x + (int)offset.x;
			if (shiftedX >= image.width)
				break;
			if (shiftedX < 0)
				continue;

			for (int y = 0; y < overlay.height; y++) {
				int shiftedY = y + (int)offset.y;
				if (shiftedY >= image.height)
					break;
				if (shiftedY < 0)
					continue;

				Color color = image.GetPixel (shiftedX, shiftedY);
				Color overlayColor = overlay.GetPixel (x, y);
				color.r = Overlay (color.r, overlayColor.r, color.a, overlayColor.a);
				color.g = Overlay (color.g, overlayColor.g, color.a, overlayColor.a);
				color.b = Overlay (color.b, overlayColor.b, color.a, overlayColor.a);
				color.a = Mathf.Min (color.a + overlayColor.a, 1);
				image.SetPixel (shiftedX, shiftedY, color);
			}
		}
		image.Apply ();
	}

	public static float Overlay (float a, float b, float aAlpha, float bAlpha)
	{
		return bAlpha * b + (1 - bAlpha) * aAlpha * a;
	}

	public static string SaveImage (Texture2D image, string relativePath, string filename)
	{
		return SaveImage (image, relativePath, filename, true);
	}

	public static string SaveImage (Texture2D image, string directory, string filename, bool isRelativePath)
	{
		if (isRelativePath)
			directory = Path.Combine (Application.persistentDataPath, directory);
		if (!Directory.Exists (directory))
			Directory.CreateDirectory (directory);
		string filePath = Path.Combine (directory, filename + ".png");
		File.WriteAllBytes (filePath, image.EncodeToPNG ());

		return filePath;
	}

	public static void DeleteImage (string relativePath, string filename)
	{
		string directory = Path.Combine (Application.persistentDataPath, relativePath);
		if (!Directory.Exists (directory))
			return;
		File.Delete (Path.Combine (directory, filename + ".png"));
	}

}
