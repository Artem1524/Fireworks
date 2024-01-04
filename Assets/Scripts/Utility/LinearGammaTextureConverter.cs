using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class LinearGammaTextureConverter : MonoBehaviour
{ // ПЕРЕД ИСПОЛЬЗОВАНИЕМ ТЕКСТУРЫ НУЖНО ПЕРЕЙТИ В НАСТРОЙКИ ТЕКСТУРЫ И ПОСТАВИТЬ ГАЛОЧКУ "Advanced -> Read/Write" !!!
    private const string SAVED_IMAGE_FILE_EXTENSION = ".png";
	private const string ALPHA_FILENAME_SUFFIX = "_alpha";
    private const string GAMMA_FILENAME_SUFFIX = "_gamma";
    private const string LINEAR_FILENAME_SUFFIX = "_linear";
    private const string FILE_EXTENSIONS_PATTERN = @"\.(?i)(bmp|gif|tif|tiff|tga|png|jpg|jpeg)$"; // Поиск расширения в имени файла ( (i?) - игнорируем регистр, (bmp|gif) - список слов для поиска bmp или gif, $ - символ завершения строки)
    private const string SAVE_FILE_COPY_SUFFIX = "_";

    private const int MAX_SAVE_FILE_ATTEMPTS = 1000;

    // private const float GAMMA_TO_LINEAR_POW_MODIFIER = 2.2f;

    [SerializeField]
    private Texture2D texture2D;

    [SerializeField]
    private LinearGammaConvertMode convertMode = LinearGammaConvertMode.ToLinear;

    public void ConvertTextureAndSaveToFile()
    {
        Texture2D newTexture = ConvertTexture(texture2D);
        string dirPath = GetDirPath();
        SaveTextureToPNGFile(newTexture, dirPath, texture2D.name);
    }

    private Texture2D ConvertTexture(Texture2D texture2D)
    {
        LogMessage($"Загружаем текстуру: {texture2D.name}");

        LogMessage($"Размер изображения: {texture2D.width} х {texture2D.height}, " +
                    $"Формат (RGB24, RGBA32...): {texture2D.format}");

        int mipmapCount = texture2D.mipmapCount;
        LogMessage($"Количество MipMap текстуры: {mipmapCount}");

        Color[] colorData = GetColorData(texture2D);

        LogMessage("Изменяем изображение...");
        Color[] newColorData = MakeChangedImageData(in colorData);

        Texture2D newTexture = new Texture2D(texture2D.width, texture2D.height, TextureFormat.RGBA32, -1, false);

        newTexture.SetPixels(newColorData);

        LogMessage($"Применяем изменения...");
        newTexture.Apply(true);

        return newTexture;

        // Color[][] colorData = GetColorDataForMipmaps(texture2D);

        /*for (int i = 0; i < mipmapCount; i++)
        {
            newTexture.SetPixels(colorData[i], i);
        }*/
    }

    private Color[] GetColorData(Texture2D texture2D)
    {
        return texture2D.GetPixels();
    }

    private Color[] MakeChangedImageData(in Color[] colorData)
    {
        switch (convertMode)
        {
            case LinearGammaConvertMode.ToLinear:
                LogMessage("Конвертируем в линейное цветовое пространство...");
                return ConvertColorDataToLinear(colorData);
            case LinearGammaConvertMode.ToGamma:
                LogMessage("Конвертируем в гамма (sRGB) цветовое пространство...");
                return ConvertColorDataToGamma(colorData);
			case LinearGammaConvertMode.CutAlphaMode:
			default:
				LogMessage("Конвертируем alpha channel...");
				return CutAlphaColorData(colorData);
        }
    }
	
	private Color[] CutAlphaColorData(in Color[] colorData)
	{
		int pixelCount = colorData.Length;
		Color[] newData = new Color[pixelCount];
		
		for (int i = 0; i < pixelCount; i++)
		{
			newData[i] = CutAlphaFromColor(colorData[i]);
		}
		
		return newData;
	}

    private Color[] ConvertColorDataToLinear(in Color[] colorData)
    {
        int pixelCount = colorData.Length;
        Color[] newData = new Color[pixelCount];

        for (int i = 0; i < pixelCount; i++)
        {
            newData[i] = ConvertColorToLinear(colorData[i]);
        }

        return newData;
    }

    private Color[] ConvertColorDataToGamma(in Color[] colorData)
    {
        int pixelCount = colorData.Length;
        Color[] newData = new Color[pixelCount];

        for (int i = 0; i < pixelCount; i++)
        {
            newData[i] = ConvertColorToGamma(colorData[i]);
        }

        return newData;
    }
	
	private Color CutAlphaFromColor(Color color)
	{
		Color newColor = new Color();
		
		newColor.r = color.r;
		newColor.g = color.g;
		newColor.b = color.b;
		
		if (color.a <= 0.93)
			newColor.a = 0;
		else
			newColor.a = 1;
		
		return newColor;
	}

    private Color ConvertColorToLinear(Color color)
    {
        return color.linear;
    }

    private Color ConvertColorToGamma(Color color)
    {
        return color.gamma;
    }

    private void SaveTextureToPNGFile(Texture2D texture, string dirPath, string fileName)
    {
        string fileNameWithSuffix = GetFileNameWithSuffix(fileName, convertMode);

        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        LogMessage("Кодируем текстуру в PNG...");
        byte[] bytes = texture.EncodeToPNG();

        bool isSuccess = TryGetNewFileName(dirPath, fileNameWithSuffix, out string newFileName); // Пытаемся найти подходящее имя файла для сохранения

        string pathToFile = dirPath + newFileName;

        LogMessage($"Сохраняем изображение в файл {pathToFile} ...");

        if (isSuccess)
            File.WriteAllBytes(pathToFile, bytes);
        else
            LogError("Не удалось найти доступное имя файла для сохранения (файлы с такими именами уже существуют)");
    }

    private string GetFileNameWithSuffix(string fileName, LinearGammaConvertMode convertMode)
    {
        switch (convertMode)
        {
            case LinearGammaConvertMode.ToLinear:
                return fileName + LINEAR_FILENAME_SUFFIX;
            case LinearGammaConvertMode.ToGamma:
                return fileName + GAMMA_FILENAME_SUFFIX;
			case LinearGammaConvertMode.CutAlphaMode:
			default:
				return fileName + ALPHA_FILENAME_SUFFIX;
        }
    }

    /// <summary>
    /// Метод, который осуществляет подбор имени файла для сохранения
    /// (если файл с таким именем существует, пытается создать файл с другим именем)
    /// Число попыток MAX_SAVE_FILE_ATTEMPTS (копии)
    /// Пример: "pic_gamma_2.png"
    /// </summary>
    /// <param name="dirPath">Путь к папке, в которую сохраняем файл</param>
    /// <param name="fileName">Желаемое имя файла для сохраненя</param>
    /// <param name="newFileName">[out] Итоговое название файла</param>
    /// <returns>True, если удалось найти подходящее имя файла для сохранения</returns>
    private bool TryGetNewFileName(string dirPath, string fileName, out string newFileName)
    {
        string newExtension = SAVED_IMAGE_FILE_EXTENSION;
        newFileName = fileName;

        if (!File.Exists(dirPath + fileName + newExtension)) // Пример имени файла: + "123_gamma" + ".png" -> "123_gamma.png"
        {
            newFileName = fileName + newExtension;
            return true;
        }

        for (int i = 1; i <= MAX_SAVE_FILE_ATTEMPTS; i++) // Пример имени файла: "pic_gamma" + "_" + "2" + ".png" -> "pic_gamma_2.png"
        {
            if (!File.Exists(dirPath + fileName + SAVE_FILE_COPY_SUFFIX + i.ToString() + newExtension))
            {
                newFileName = fileName + SAVE_FILE_COPY_SUFFIX + i.ToString() + newExtension;
                return true;
            }
        }

        return false; // Если не получается найти имя файла (существуют все файлы с такими именами)

        // fileNameWithoutExtension = GetFileNameWithoutExtension(fileName);
    }

    private string GetDirPath()
    {
        string dirPath = Application.dataPath + "/ConvertedTextures/";
        return dirPath;
    }

    private void LogMessage(string message)
    {
#if DEBUG
        Debug.Log(message);
#endif
    }

    private void LogError(string message)
    {
#if DEBUG
        Debug.LogError(message);
#endif
    }

    /*private Color ConvertColorToLinear(Color color)
    {
        float PARTICLE_TINT_COEFFICIENT = 0.7f;
        return new Color(Mathf.Pow(color.r, GAMMA_TO_LINEAR_POW_MODIFIER) * PARTICLE_TINT_COEFFICIENT,
                            Mathf.Pow(color.g, GAMMA_TO_LINEAR_POW_MODIFIER) * PARTICLE_TINT_COEFFICIENT,
                            Mathf.Pow(color.b, GAMMA_TO_LINEAR_POW_MODIFIER) * PARTICLE_TINT_COEFFICIENT,
                            Mathf.Pow(color.a, GAMMA_TO_LINEAR_POW_MODIFIER) * PARTICLE_TINT_COEFFICIENT);
    }*/

    /*private float NormalRasprTranslator(float x)
    {
        float rez = 1 - NormalRaspr(x);

        if (rez <= 0.04)
            rez = 0;
        return rez;
    }*/

    /*private Color NormalRasprColor(Color color) Делает изображение более чётким
    {
        float r = NormalRasprTranslator(color.r);
        float g = NormalRasprTranslator(color.g);
        float b = NormalRasprTranslator(color.b);
        LogMessage(string.Format("R={0:f2} , G={0:f2} , B={0:f2} , Rn={0:f2} , Gn={0:f2} , Bn={0:f2}", color.r, color.g, color.b, r, g, b));
        r = Mathf.Clamp01(r);
        g = Mathf.Clamp01(g);
        b = Mathf.Clamp01(b);
        Color newColor = new Color(r, g, b, color.a);
        // Mathf.GammaToLinearSpace(color.r)
        return newColor;
        //return color.linear;
    }*/

    /*private float NormalRaspr(float x)
    {
        float DELTA = 0.4f;
        float TWODELTAONDELTA = 2f * DELTA * DELTA;
        return Mathf.Exp((-x * x) / TWODELTAONDELTA); // x = 0 y = 1 x = 1 y ~ 0.04 x = 0.5 y ~ 0.46

        // return 1 / (delta * Mathf.Sqrt(2 * Mathf.PI)) * Mathf.Exp((-x * x) / (2 * delta * delta))
        // Преобразуется в Mathf.Exp((-x * x) / (2 * delta * delta)), так как 1 / (0.4f * Mathf.Sqrt(2 * Mathf.PI)) ~ 1
    }*/

    /*private string GetFileNameWithoutExtension(string fileName)
    {
        Match extensionMatch = Regex.Match(fileName, FILE_EXTENSIONS_PATTERN);

        if (extensionMatch.Success) // Если находим расширение в имени файла, то удаляем его (Пример: "123.bmp" -> "123")
        {
            Group preLastGroup = extensionMatch.Groups[extensionMatch.Groups.Count - 2];
            return fileName.Substring(0, preLastGroup.Index);
        }

        return fileName;
    }*/

    /*private Color[][] GetColorDataForMipmaps(Texture2D texture2D)
    {
        int mipmapCount = texture2D.mipmapCount;

        Color[][] colorMipmapsData = new Color[mipmapCount][];

        for (int i = 1; i <= mipmapCount; i++)
        {
            LogMessage($"Mipmap уровень: {i}");
            colorMipmapsData[i] = texture2D.GetPixels(i);
            LogMessage($"Количество пикселей: {colorMipmapsData[i].Length}");
        }

        return colorMipmapsData;
    }*/
}
