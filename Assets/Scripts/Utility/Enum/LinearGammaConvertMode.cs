using System;

[Serializable]
public enum LinearGammaConvertMode : byte
{
    ToLinear = 1,
    ToGamma = 2,
	CutAlphaMode = 3
}
