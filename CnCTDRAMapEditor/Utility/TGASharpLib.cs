﻿//
// Copyright 2020 Electronic Arts Inc.
//
// The Command & Conquer Map Editor and corresponding source code is free 
// software: you can redistribute it and/or modify it under the terms of 
// the GNU General Public License as published by the Free Software Foundation, 
// either version 3 of the License, or (at your option) any later version.

// The Command & Conquer Map Editor and corresponding source code is distributed 
// in the hope that it will be useful, but with permitted additional restrictions 
// under Section 7 of the GPL. See the GNU General Public License in LICENSE.TXT 
// distributed with this program. You should have received a copy of the 
// GNU General Public License along with permitted additional restrictions 
// with this program. If not, see https://github.com/electronicarts/CnC_Remastered_Collection
/*                         MIT License
                 Copyright (c) 2017 TGASharpLib

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TGASharpLib {
    #region Enums
    /// <summary>
    /// <para>The first 128 Color Map Type codes are reserved for use by Truevision,
    /// while the second set of 128 Color Map Type codes(128 to 255) may be used for
    /// developer applications.</para>
    /// True-Color images do not normally make use of the color map field, but some current
    /// applications store palette information or developer-defined information in this field.
    /// It is best to check Field 3, Image Type, to make sure you have a file which can use the
    /// data stored in the Color Map Field.
    /// Otherwise ignore the information. When saving or creating files for True-Color
    /// images do not use this field and set it to Zero to ensure compatibility. Please refer
    /// to the Developer Area specification for methods of storing developer defined information.
    /// </summary>
    public enum TgaColorMapType : byte {
        NoColorMap = 0,
        ColorMap = 1,
        Truevision_2,
        Truevision_3,
        Truevision_4,
        Truevision_5,
        Truevision_6,
        Truevision_7,
        Truevision_8,
        Truevision_9,
        Truevision_10,
        Truevision_11,
        Truevision_12,
        Truevision_13,
        Truevision_14,
        Truevision_15,
        Truevision_16,
        Truevision_17,
        Truevision_18,
        Truevision_19,
        Truevision_20,
        Truevision_21,
        Truevision_22,
        Truevision_23,
        Truevision_24,
        Truevision_25,
        Truevision_26,
        Truevision_27,
        Truevision_28,
        Truevision_29,
        Truevision_30,
        Truevision_31,
        Truevision_32,
        Truevision_33,
        Truevision_34,
        Truevision_35,
        Truevision_36,
        Truevision_37,
        Truevision_38,
        Truevision_39,
        Truevision_40,
        Truevision_41,
        Truevision_42,
        Truevision_43,
        Truevision_44,
        Truevision_45,
        Truevision_46,
        Truevision_47,
        Truevision_48,
        Truevision_49,
        Truevision_50,
        Truevision_51,
        Truevision_52,
        Truevision_53,
        Truevision_54,
        Truevision_55,
        Truevision_56,
        Truevision_57,
        Truevision_58,
        Truevision_59,
        Truevision_60,
        Truevision_61,
        Truevision_62,
        Truevision_63,
        Truevision_64,
        Truevision_65,
        Truevision_66,
        Truevision_67,
        Truevision_68,
        Truevision_69,
        Truevision_70,
        Truevision_71,
        Truevision_72,
        Truevision_73,
        Truevision_74,
        Truevision_75,
        Truevision_76,
        Truevision_77,
        Truevision_78,
        Truevision_79,
        Truevision_80,
        Truevision_81,
        Truevision_82,
        Truevision_83,
        Truevision_84,
        Truevision_85,
        Truevision_86,
        Truevision_87,
        Truevision_88,
        Truevision_89,
        Truevision_90,
        Truevision_91,
        Truevision_92,
        Truevision_93,
        Truevision_94,
        Truevision_95,
        Truevision_96,
        Truevision_97,
        Truevision_98,
        Truevision_99,
        Truevision_100,
        Truevision_101,
        Truevision_102,
        Truevision_103,
        Truevision_104,
        Truevision_105,
        Truevision_106,
        Truevision_107,
        Truevision_108,
        Truevision_109,
        Truevision_110,
        Truevision_111,
        Truevision_112,
        Truevision_113,
        Truevision_114,
        Truevision_115,
        Truevision_116,
        Truevision_117,
        Truevision_118,
        Truevision_119,
        Truevision_120,
        Truevision_121,
        Truevision_122,
        Truevision_123,
        Truevision_124,
        Truevision_125,
        Truevision_126,
        Truevision_127,
        Other_128,
        Other_129,
        Other_130,
        Other_131,
        Other_132,
        Other_133,
        Other_134,
        Other_135,
        Other_136,
        Other_137,
        Other_138,
        Other_139,
        Other_140,
        Other_141,
        Other_142,
        Other_143,
        Other_144,
        Other_145,
        Other_146,
        Other_147,
        Other_148,
        Other_149,
        Other_150,
        Other_151,
        Other_152,
        Other_153,
        Other_154,
        Other_155,
        Other_156,
        Other_157,
        Other_158,
        Other_159,
        Other_160,
        Other_161,
        Other_162,
        Other_163,
        Other_164,
        Other_165,
        Other_166,
        Other_167,
        Other_168,
        Other_169,
        Other_170,
        Other_171,
        Other_172,
        Other_173,
        Other_174,
        Other_175,
        Other_176,
        Other_177,
        Other_178,
        Other_179,
        Other_180,
        Other_181,
        Other_182,
        Other_183,
        Other_184,
        Other_185,
        Other_186,
        Other_187,
        Other_188,
        Other_189,
        Other_190,
        Other_191,
        Other_192,
        Other_193,
        Other_194,
        Other_195,
        Other_196,
        Other_197,
        Other_198,
        Other_199,
        Other_200,
        Other_201,
        Other_202,
        Other_203,
        Other_204,
        Other_205,
        Other_206,
        Other_207,
        Other_208,
        Other_209,
        Other_210,
        Other_211,
        Other_212,
        Other_213,
        Other_214,
        Other_215,
        Other_216,
        Other_217,
        Other_218,
        Other_219,
        Other_220,
        Other_221,
        Other_222,
        Other_223,
        Other_224,
        Other_225,
        Other_226,
        Other_227,
        Other_228,
        Other_229,
        Other_230,
        Other_231,
        Other_232,
        Other_233,
        Other_234,
        Other_235,
        Other_236,
        Other_237,
        Other_238,
        Other_239,
        Other_240,
        Other_241,
        Other_242,
        Other_243,
        Other_244,
        Other_245,
        Other_246,
        Other_247,
        Other_248,
        Other_249,
        Other_250,
        Other_251,
        Other_252,
        Other_253,
        Other_254,
        Other_255
    }

    /// <summary>
    /// Establishes the number of bits per entry. Typically 15, 16, 24 or 32-bit values are used.
    /// <para>When working with VDA or VDA/D cards it is preferred that you select 16 bits(5 bits
    /// per primary with 1 bit to select interrupt control) and set the 16th bit to 0 so that the
    /// interrupt bit is disabled. Even if this field is set to 15 bits(5 bits per primary) you
    /// must still parse the color map data 16 bits at a time and ignore the 16th bit.</para>
    /// <para>When working with a TARGA M8 card you would select 24 bits (8 bits per primary)
    /// since the color map is defined as 256 entries of 24 bit color values.</para>
    /// When working with a TrueVista card(ATVista or NuVista) you would select 24-bit(8 bits per
    /// primary) or 32-bit(8 bits per primary including Alpha channel) depending on your
    /// application’s use of look-up tables. It is suggested that when working with 16-bit and
    /// 32-bit color images, you store them as True-Color images and do not use the color map 
    /// field to store look-up tables. Please refer to the TGA Extensions for fields better suited
    /// to storing look-up table information.
    /// </summary>
    public enum TgaColorMapEntrySize : byte {
        Other = 0,
        X1R5G5B5 = 15,
        A1R5G5B5 = 16,
        R8G8B8 = 24,
        A8R8G8B8 = 32
    }

    /// <summary>
    /// Truevision has currently defined seven image types:
    /// <para>0 - No Image Data Included;</para>
    /// <para>1 - Uncompressed, Color-mapped Image;</para>
    /// <para>2 - Uncompressed, True-color Image;</para>
    /// <para>3 - Uncompressed, Black-and-white Image;</para>
    /// <para>9 - Run-length encoded, Color-mapped Image;</para>
    /// <para>10 - Run-length encoded, True-color Image;</para>
    /// <para>11 - Run-length encoded, Black-and-white Image.</para>
    /// Image Data Type codes 0 to 127 are reserved for use by Truevision for general applications.
    /// Image Data Type codes 128 to 255 may be used for developer applications.
    /// </summary>
    public enum TgaImageType : byte {
        NoImageData = 0,
        Uncompressed_ColorMapped = 1,
        Uncompressed_TrueColor,
        Uncompressed_BlackWhite,
        _Truevision_4,
        _Truevision_5,
        _Truevision_6,
        _Truevision_7,
        _Truevision_8,
        RLE_ColorMapped = 9,
        RLE_TrueColor,
        RLE_BlackWhite,
        _Truevision_12,
        _Truevision_13,
        _Truevision_14,
        _Truevision_15,
        _Truevision_16,
        _Truevision_17,
        _Truevision_18,
        _Truevision_19,
        _Truevision_20,
        _Truevision_21,
        _Truevision_22,
        _Truevision_23,
        _Truevision_24,
        _Truevision_25,
        _Truevision_26,
        _Truevision_27,
        _Truevision_28,
        _Truevision_29,
        _Truevision_30,
        _Truevision_31,
        _Truevision_32,
        _Truevision_33,
        _Truevision_34,
        _Truevision_35,
        _Truevision_36,
        _Truevision_37,
        _Truevision_38,
        _Truevision_39,
        _Truevision_40,
        _Truevision_41,
        _Truevision_42,
        _Truevision_43,
        _Truevision_44,
        _Truevision_45,
        _Truevision_46,
        _Truevision_47,
        _Truevision_48,
        _Truevision_49,
        _Truevision_50,
        _Truevision_51,
        _Truevision_52,
        _Truevision_53,
        _Truevision_54,
        _Truevision_55,
        _Truevision_56,
        _Truevision_57,
        _Truevision_58,
        _Truevision_59,
        _Truevision_60,
        _Truevision_61,
        _Truevision_62,
        _Truevision_63,
        _Truevision_64,
        _Truevision_65,
        _Truevision_66,
        _Truevision_67,
        _Truevision_68,
        _Truevision_69,
        _Truevision_70,
        _Truevision_71,
        _Truevision_72,
        _Truevision_73,
        _Truevision_74,
        _Truevision_75,
        _Truevision_76,
        _Truevision_77,
        _Truevision_78,
        _Truevision_79,
        _Truevision_80,
        _Truevision_81,
        _Truevision_82,
        _Truevision_83,
        _Truevision_84,
        _Truevision_85,
        _Truevision_86,
        _Truevision_87,
        _Truevision_88,
        _Truevision_89,
        _Truevision_90,
        _Truevision_91,
        _Truevision_92,
        _Truevision_93,
        _Truevision_94,
        _Truevision_95,
        _Truevision_96,
        _Truevision_97,
        _Truevision_98,
        _Truevision_99,
        _Truevision_100,
        _Truevision_101,
        _Truevision_102,
        _Truevision_103,
        _Truevision_104,
        _Truevision_105,
        _Truevision_106,
        _Truevision_107,
        _Truevision_108,
        _Truevision_109,
        _Truevision_110,
        _Truevision_111,
        _Truevision_112,
        _Truevision_113,
        _Truevision_114,
        _Truevision_115,
        _Truevision_116,
        _Truevision_117,
        _Truevision_118,
        _Truevision_119,
        _Truevision_120,
        _Truevision_121,
        _Truevision_122,
        _Truevision_123,
        _Truevision_124,
        _Truevision_125,
        _Truevision_126,
        _Truevision_127,
        _Other_128,
        _Other_129,
        _Other_130,
        _Other_131,
        _Other_132,
        _Other_133,
        _Other_134,
        _Other_135,
        _Other_136,
        _Other_137,
        _Other_138,
        _Other_139,
        _Other_140,
        _Other_141,
        _Other_142,
        _Other_143,
        _Other_144,
        _Other_145,
        _Other_146,
        _Other_147,
        _Other_148,
        _Other_149,
        _Other_150,
        _Other_151,
        _Other_152,
        _Other_153,
        _Other_154,
        _Other_155,
        _Other_156,
        _Other_157,
        _Other_158,
        _Other_159,
        _Other_160,
        _Other_161,
        _Other_162,
        _Other_163,
        _Other_164,
        _Other_165,
        _Other_166,
        _Other_167,
        _Other_168,
        _Other_169,
        _Other_170,
        _Other_171,
        _Other_172,
        _Other_173,
        _Other_174,
        _Other_175,
        _Other_176,
        _Other_177,
        _Other_178,
        _Other_179,
        _Other_180,
        _Other_181,
        _Other_182,
        _Other_183,
        _Other_184,
        _Other_185,
        _Other_186,
        _Other_187,
        _Other_188,
        _Other_189,
        _Other_190,
        _Other_191,
        _Other_192,
        _Other_193,
        _Other_194,
        _Other_195,
        _Other_196,
        _Other_197,
        _Other_198,
        _Other_199,
        _Other_200,
        _Other_201,
        _Other_202,
        _Other_203,
        _Other_204,
        _Other_205,
        _Other_206,
        _Other_207,
        _Other_208,
        _Other_209,
        _Other_210,
        _Other_211,
        _Other_212,
        _Other_213,
        _Other_214,
        _Other_215,
        _Other_216,
        _Other_217,
        _Other_218,
        _Other_219,
        _Other_220,
        _Other_221,
        _Other_222,
        _Other_223,
        _Other_224,
        _Other_225,
        _Other_226,
        _Other_227,
        _Other_228,
        _Other_229,
        _Other_230,
        _Other_231,
        _Other_232,
        _Other_233,
        _Other_234,
        _Other_235,
        _Other_236,
        _Other_237,
        _Other_238,
        _Other_239,
        _Other_240,
        _Other_241,
        _Other_242,
        _Other_243,
        _Other_244,
        _Other_245,
        _Other_246,
        _Other_247,
        _Other_248,
        _Other_249,
        _Other_250,
        _Other_251,
        _Other_252,
        _Other_253,
        _Other_254,
        _Other_255
    }

    /// <summary>
    /// Number of bits per pixel. This number includes the Attribute or Alpha channel bits.
    /// Common values are 8, 16, 24 and 32 but other pixel depths could be used.
    /// </summary>
    public enum TgaPixelDepth : byte {
        Other = 0,
        Bpp8 = 8,
        Bpp16 = 16,
        Bpp24 = 24,
        Bpp32 = 32
    }

    /// <summary>
    /// Used to indicate the order in which pixel data is transferred from the file to the screen.
    /// (Bit 4 (bit 0 in enum) is for left-to-right ordering and bit 5 (bit 1 in enum) is for
    /// topto-bottom ordering as shown below.)
    /// </summary>
    public enum TgaImgOrigin : byte {
        BottomLeft = 0,
        BottomRight,
        TopLeft,
        TopRight
    }

    /// <summary>
    /// Contains a value which specifies the type of Alpha channel
    /// data contained in the file. Value Meaning:
    /// <para>0: no Alpha data included (bits 3-0 of field 5.6 should also be set to zero)</para>
    /// <para>1: undefined data in the Alpha field, can be ignored</para>
    /// <para>2: undefined data in the Alpha field, but should be retained</para>
    /// <para>3: useful Alpha channel data is present</para>
    /// <para>4: pre-multiplied Alpha(see description below)</para>
    /// <para>5 -127: RESERVED</para>
    /// <para>128-255: Un-assigned</para>
    /// <para>Pre-multiplied Alpha Example: Suppose the Alpha channel data is being used to specify the
    /// opacity of each pixel(for use when the image is overlayed on another image), where 0 indicates
    /// that the pixel is completely transparent and a value of 1 indicates that the pixel is
    /// completely opaque(assume all component values have been normalized).</para>
    /// <para>A quadruple(a, r, g, b) of( 0.5, 1, 0, 0) would indicate that the pixel is pure red with a
    /// transparency of one-half. For numerous reasons(including image compositing) is is better to
    /// pre-multiply the individual color components with the value in the Alpha channel.</para>
    /// A pre-multiplication of the above would produce a quadruple(0.5, 0.5, 0, 0).
    /// A value of 3 in the Attributes Type Field(field 23) would indicate that the color components
    /// of the pixel have already been scaled by the value in the Alpha channel.
    /// </summary>
    public enum TgaAttrType : byte {
        NoAlpha = 0,
        UndefinedAlphaCanBeIgnored,
        UndefinedAlphaButShouldBeRetained,
        UsefulAlpha,
        PreMultipliedAlpha,
        _Reserved_5,
        _Reserved_6,
        _Reserved_7,
        _Reserved_8,
        _Reserved_9,
        _Reserved_10,
        _Reserved_11,
        _Reserved_12,
        _Reserved_13,
        _Reserved_14,
        _Reserved_15,
        _Reserved_16,
        _Reserved_17,
        _Reserved_18,
        _Reserved_19,
        _Reserved_20,
        _Reserved_21,
        _Reserved_22,
        _Reserved_23,
        _Reserved_24,
        _Reserved_25,
        _Reserved_26,
        _Reserved_27,
        _Reserved_28,
        _Reserved_29,
        _Reserved_30,
        _Reserved_31,
        _Reserved_32,
        _Reserved_33,
        _Reserved_34,
        _Reserved_35,
        _Reserved_36,
        _Reserved_37,
        _Reserved_38,
        _Reserved_39,
        _Reserved_40,
        _Reserved_41,
        _Reserved_42,
        _Reserved_43,
        _Reserved_44,
        _Reserved_45,
        _Reserved_46,
        _Reserved_47,
        _Reserved_48,
        _Reserved_49,
        _Reserved_50,
        _Reserved_51,
        _Reserved_52,
        _Reserved_53,
        _Reserved_54,
        _Reserved_55,
        _Reserved_56,
        _Reserved_57,
        _Reserved_58,
        _Reserved_59,
        _Reserved_60,
        _Reserved_61,
        _Reserved_62,
        _Reserved_63,
        _Reserved_64,
        _Reserved_65,
        _Reserved_66,
        _Reserved_67,
        _Reserved_68,
        _Reserved_69,
        _Reserved_70,
        _Reserved_71,
        _Reserved_72,
        _Reserved_73,
        _Reserved_74,
        _Reserved_75,
        _Reserved_76,
        _Reserved_77,
        _Reserved_78,
        _Reserved_79,
        _Reserved_80,
        _Reserved_81,
        _Reserved_82,
        _Reserved_83,
        _Reserved_84,
        _Reserved_85,
        _Reserved_86,
        _Reserved_87,
        _Reserved_88,
        _Reserved_89,
        _Reserved_90,
        _Reserved_91,
        _Reserved_92,
        _Reserved_93,
        _Reserved_94,
        _Reserved_95,
        _Reserved_96,
        _Reserved_97,
        _Reserved_98,
        _Reserved_99,
        _Reserved_100,
        _Reserved_101,
        _Reserved_102,
        _Reserved_103,
        _Reserved_104,
        _Reserved_105,
        _Reserved_106,
        _Reserved_107,
        _Reserved_108,
        _Reserved_109,
        _Reserved_110,
        _Reserved_111,
        _Reserved_112,
        _Reserved_113,
        _Reserved_114,
        _Reserved_115,
        _Reserved_116,
        _Reserved_117,
        _Reserved_118,
        _Reserved_119,
        _Reserved_120,
        _Reserved_121,
        _Reserved_122,
        _Reserved_123,
        _Reserved_124,
        _Reserved_125,
        _Reserved_126,
        _Reserved_127,
        _UnAssigned_128,
        _UnAssigned_129,
        _UnAssigned_130,
        _UnAssigned_131,
        _UnAssigned_132,
        _UnAssigned_133,
        _UnAssigned_134,
        _UnAssigned_135,
        _UnAssigned_136,
        _UnAssigned_137,
        _UnAssigned_138,
        _UnAssigned_139,
        _UnAssigned_140,
        _UnAssigned_141,
        _UnAssigned_142,
        _UnAssigned_143,
        _UnAssigned_144,
        _UnAssigned_145,
        _UnAssigned_146,
        _UnAssigned_147,
        _UnAssigned_148,
        _UnAssigned_149,
        _UnAssigned_150,
        _UnAssigned_151,
        _UnAssigned_152,
        _UnAssigned_153,
        _UnAssigned_154,
        _UnAssigned_155,
        _UnAssigned_156,
        _UnAssigned_157,
        _UnAssigned_158,
        _UnAssigned_159,
        _UnAssigned_160,
        _UnAssigned_161,
        _UnAssigned_162,
        _UnAssigned_163,
        _UnAssigned_164,
        _UnAssigned_165,
        _UnAssigned_166,
        _UnAssigned_167,
        _UnAssigned_168,
        _UnAssigned_169,
        _UnAssigned_170,
        _UnAssigned_171,
        _UnAssigned_172,
        _UnAssigned_173,
        _UnAssigned_174,
        _UnAssigned_175,
        _UnAssigned_176,
        _UnAssigned_177,
        _UnAssigned_178,
        _UnAssigned_179,
        _UnAssigned_180,
        _UnAssigned_181,
        _UnAssigned_182,
        _UnAssigned_183,
        _UnAssigned_184,
        _UnAssigned_185,
        _UnAssigned_186,
        _UnAssigned_187,
        _UnAssigned_188,
        _UnAssigned_189,
        _UnAssigned_190,
        _UnAssigned_191,
        _UnAssigned_192,
        _UnAssigned_193,
        _UnAssigned_194,
        _UnAssigned_195,
        _UnAssigned_196,
        _UnAssigned_197,
        _UnAssigned_198,
        _UnAssigned_199,
        _UnAssigned_200,
        _UnAssigned_201,
        _UnAssigned_202,
        _UnAssigned_203,
        _UnAssigned_204,
        _UnAssigned_205,
        _UnAssigned_206,
        _UnAssigned_207,
        _UnAssigned_208,
        _UnAssigned_209,
        _UnAssigned_210,
        _UnAssigned_211,
        _UnAssigned_212,
        _UnAssigned_213,
        _UnAssigned_214,
        _UnAssigned_215,
        _UnAssigned_216,
        _UnAssigned_217,
        _UnAssigned_218,
        _UnAssigned_219,
        _UnAssigned_220,
        _UnAssigned_221,
        _UnAssigned_222,
        _UnAssigned_223,
        _UnAssigned_224,
        _UnAssigned_225,
        _UnAssigned_226,
        _UnAssigned_227,
        _UnAssigned_228,
        _UnAssigned_229,
        _UnAssigned_230,
        _UnAssigned_231,
        _UnAssigned_232,
        _UnAssigned_233,
        _UnAssigned_234,
        _UnAssigned_235,
        _UnAssigned_236,
        _UnAssigned_237,
        _UnAssigned_238,
        _UnAssigned_239,
        _UnAssigned_240,
        _UnAssigned_241,
        _UnAssigned_242,
        _UnAssigned_243,
        _UnAssigned_244,
        _UnAssigned_245,
        _UnAssigned_246,
        _UnAssigned_247,
        _UnAssigned_248,
        _UnAssigned_249,
        _UnAssigned_250,
        _UnAssigned_251,
        _UnAssigned_252,
        _UnAssigned_253,
        _UnAssigned_254,
        _UnAssigned_255
    }
    #endregion

    #region Classes
    public class TgaColorKey : ICloneable {
        private byte a = 0;
        private byte r = 0;
        private byte g = 0;
        private byte b = 0;

        public TgaColorKey() {
        }

        /// <summary>
        /// Make <see cref="TgaColorKey"/> from ARGB bytes.
        /// </summary>
        /// <param name="A">Alpha value.</param>
        /// <param name="R">Red value.</param>
        /// <param name="G">Green value.</param>
        /// <param name="B">Blue value.</param>
        public TgaColorKey(byte A, byte R, byte G, byte B) {
            this.a = A;
            this.r = R;
            this.g = G;
            this.b = B;
        }

        /// <summary>
        /// Make <see cref="TgaColorKey"/> from ARGB bytes.
        /// </summary>
        /// <param name="Bytes">Array of bytes(byte[4]).</param>
        public TgaColorKey(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            if(Bytes.Length != Size)
                throw new ArgumentOutOfRangeException(nameof(Bytes.Length) + " must be equal " + Size + "!");

            var color = Color.FromArgb(BitConverter.ToInt32(Bytes, 0));
            this.a = color.A;
            this.r = color.R;
            this.g = color.G;
            this.b = color.B;
        }

        /// <summary>
        /// Make <see cref="TgaColorKey"/> from <see cref="int"/>.
        /// </summary>
        /// <param name="ARGB">32bit ARGB integer color value.</param>
        public TgaColorKey(int ARGB) {
            var ColorARGB = Color.FromArgb(ARGB);
            this.a = ColorARGB.A;
            this.r = ColorARGB.R;
            this.g = ColorARGB.G;
            this.b = ColorARGB.B;
        }

        /// <summary>
        /// Make <see cref="TgaColorKey"/> from <see cref="Color"/>.
        /// </summary>
        /// <param name="color">GDI+ <see cref="Color"/> value.</param>
        public TgaColorKey(Color color) {
            this.a = color.A;
            this.r = color.R;
            this.g = color.G;
            this.b = color.B;
        }

        /// <summary>
        /// Gets or sets alpha color value.
        /// </summary>
        public byte A {
            get => this.a;
            set => this.a = value;
        }

        /// <summary>
        /// Gets or sets red color value.
        /// </summary>
        public byte R {
            get => this.r;
            set => this.r = value;
        }

        /// <summary>
        /// Gets or sets green color value.
        /// </summary>
        public byte G {
            get => this.g;
            set => this.g = value;
        }

        /// <summary>
        /// Gets or sets blue color value.
        /// </summary>
        public byte B {
            get => this.b;
            set => this.b = value;
        }

        /// <summary>
        /// Gets TGA Field size in bytes.
        /// </summary>
        public const int Size = 4;

        /// <summary>
        /// Make full independed copy of <see cref="TgaColorKey"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaColorKey"/></returns>
        public TgaColorKey Clone() => new TgaColorKey(this.a, this.r, this.g, this.b);

        /// <summary>
        /// Make full independed copy of <see cref="TgaColorKey"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaColorKey"/></returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaColorKey) ? this.Equals((TgaColorKey)obj) : false);

        public bool Equals(TgaColorKey item) => (this.a == item.a && this.r == item.r && this.g == item.g && this.b == item.b);

        public static bool operator ==(TgaColorKey item1, TgaColorKey item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaColorKey item1, TgaColorKey item2) => !(item1 == item2);

        public override int GetHashCode() => this.ToInt().GetHashCode();

        /// <summary>
        /// Gets <see cref="TgaColorKey"/> like string.
        /// </summary>
        /// <returns>String in ARGB format.</returns>
        public override string ToString() => string.Format("{0}={1}, {2}={3}, {4}={5}, {6}={7}",
                nameof(this.A), this.a, nameof(this.R), this.r, nameof(this.G), this.g, nameof(this.B), this.b);

        /// <summary>
        /// Convert <see cref="TgaColorKey"/> to byte array.
        /// </summary>
        /// <returns>Byte array with length = 4.</returns>
        public byte[] ToBytes() => BitConverter.GetBytes(this.ToInt());

        /// <summary>
        /// Gets <see cref="TgaColorKey"/> like GDI+ <see cref="Color"/>.
        /// </summary>
        /// <returns><see cref="Color"/> value of <see cref="TgaColorKey"/>.</returns>
        public Color ToColor() => Color.FromArgb(this.a, this.r, this.g, this.b);

        /// <summary>
        /// Gets <see cref="TgaColorKey"/> like ARGB <see cref="int"/>.
        /// </summary>
        /// <returns>ARGB <see cref="int"/> value of <see cref="TgaColorKey"/>.</returns>
        public int ToInt() => this.ToColor().ToArgb();
    }

    /// <summary>
    /// This field (5 bytes) and its sub-fields describe the color map (if any) used for the image.
    /// If the Color Map Type field is set to zero, indicating that no color map exists, then
    /// these 5 bytes should be set to zero. These bytes always must be written to the file.
    /// </summary>
    public class TgaColorMapSpec : ICloneable {
        private ushort firstEntryIndex = 0;
        private ushort colorMapLength = 0;
        private TgaColorMapEntrySize colorMapEntrySize = TgaColorMapEntrySize.Other;

        /// <summary>
        /// Make new <see cref="TgaColorMapSpec"/>.
        /// </summary>
        public TgaColorMapSpec() {
        }

        /// <summary>
        /// Make <see cref="TgaColorMapSpec"/> from bytes.
        /// </summary>
        /// <param name="Bytes">Array of bytes(byte[5]).</param>
        public TgaColorMapSpec(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            if(Bytes.Length != Size)
                throw new ArgumentOutOfRangeException(nameof(Bytes.Length) + " must be equal " + Size + "!");

            this.firstEntryIndex = BitConverter.ToUInt16(Bytes, 0);
            this.colorMapLength = BitConverter.ToUInt16(Bytes, 2);
            this.colorMapEntrySize = (TgaColorMapEntrySize)Bytes[4];
        }

        /// <summary>
        /// Field 4.1 (2 bytes):
        /// Index of the first color map entry. Index refers to the starting entry in loading
        /// the color map.
        /// <para>Example: If you would have 1024 entries in the entire color map but you only
        /// need to store 72 of those entries, this field allows you to start in the middle of
        /// the color-map (e.g., position 342).</para>
        /// </summary>
        public ushort FirstEntryIndex {
            get => this.firstEntryIndex;
            set => this.firstEntryIndex = value;
        }

        /// <summary>
        /// Field 4.2 (2 bytes):
        /// Total number of color map entries included.
        /// </summary>
        public ushort ColorMapLength {
            get => this.colorMapLength;
            set => this.colorMapLength = value;
        }

        /// <summary>
        /// Field 4.3 (1 byte):
        /// Establishes the number of bits per entry. Typically 15, 16, 24 or 32-bit values are used.
        /// <para>When working with VDA or VDA/D cards it is preferred that you select 16 bits(5 bits
        /// per primary with 1 bit to select interrupt control) and set the 16th bit to 0 so that the
        /// interrupt bit is disabled. Even if this field is set to 15 bits(5 bits per primary) you
        /// must still parse the color map data 16 bits at a time and ignore the 16th bit.</para>
        /// <para>When working with a TARGA M8 card you would select 24 bits (8 bits per primary)
        /// since the color map is defined as 256 entries of 24 bit color values.</para>
        /// When working with a TrueVista card(ATVista or NuVista) you would select 24-bit(8 bits per
        /// primary) or 32-bit(8 bits per primary including Alpha channel) depending on your
        /// application’s use of look-up tables. It is suggested that when working with 16-bit and
        /// 32-bit color images, you store them as True-Color images and do not use the color map 
        /// field to store look-up tables. Please refer to the TGA Extensions for fields better suited
        /// to storing look-up table information.
        /// </summary>
        public TgaColorMapEntrySize ColorMapEntrySize {
            get => this.colorMapEntrySize;
            set => this.colorMapEntrySize = value;
        }

        /// <summary>
        /// Gets TGA Field size in bytes.
        /// </summary>
        public const int Size = 5;

        /// <summary>
        /// Make full independed copy of <see cref="TgaColorMapSpec"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaColorMapSpec"/></returns>
        public TgaColorMapSpec Clone() => new TgaColorMapSpec(this.ToBytes());

        /// <summary>
        /// Make full independed copy of <see cref="TgaColorMapSpec"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaColorMapSpec"/></returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaColorMapSpec) ? this.Equals((TgaColorMapSpec)obj) : false);

        public bool Equals(TgaColorMapSpec item) => (this.firstEntryIndex == item.firstEntryIndex &&
                this.colorMapLength == item.colorMapLength &&
                this.colorMapEntrySize == item.colorMapEntrySize);

        public static bool operator ==(TgaColorMapSpec item1, TgaColorMapSpec item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaColorMapSpec item1, TgaColorMapSpec item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                return (this.firstEntryIndex << 16 | this.colorMapLength).GetHashCode() ^ this.colorMapEntrySize.GetHashCode();
            }
        }

        public override string ToString() => string.Format("{0}={1}, {2}={3}, {4}={5}", nameof(this.FirstEntryIndex), this.FirstEntryIndex,
                nameof(this.ColorMapLength), this.ColorMapLength, nameof(this.ColorMapEntrySize), this.ColorMapEntrySize);

        /// <summary>
        /// Convert ColorMapSpec to byte array.
        /// </summary>
        /// <returns>Byte array with length = 5.</returns>
        public byte[] ToBytes() => BitConverterExt.ToBytes(this.firstEntryIndex, this.colorMapLength, (byte)this.colorMapEntrySize);
    }

    public class TgaComment : ICloneable {
        private const int StrNLen = 80; //80 ASCII chars + 1 '\0' = 81 per SrtN!
        private string origString = string.Empty;
        private char blankSpaceChar = TgaString.DefaultBlankSpaceChar;

        public TgaComment() {
        }

        public TgaComment(string Str, char BlankSpaceChar = '\0') {
            if(Str == null)
                throw new ArgumentNullException(nameof(Str) + " = null!");

            this.origString = Str;
            this.blankSpaceChar = BlankSpaceChar;
        }

        public TgaComment(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            if(Bytes.Length != Size)
                throw new ArgumentOutOfRangeException(nameof(Bytes.Length) + " must be equal " + Size + "!");

            var s = Encoding.ASCII.GetString(Bytes, 0, StrNLen);
            s += Encoding.ASCII.GetString(Bytes, 81, StrNLen);
            s += Encoding.ASCII.GetString(Bytes, 162, StrNLen);
            s += Encoding.ASCII.GetString(Bytes, 243, StrNLen);

            switch(s[s.Length - 1]) {
            case '\0':
            case ' ':
                this.blankSpaceChar = s[s.Length - 1];
                this.origString = s.TrimEnd(new char[] { s[s.Length - 1] });
                break;
            default:
                this.origString = s;
                break;
            }
        }

        /// <summary>
        /// Gets TGA Field size in bytes.
        /// </summary>
        public const int Size = 81 * 4;

        public string OriginalString {
            get => this.origString;
            set => this.origString = value;
        }

        public char BlankSpaceChar {
            get => this.blankSpaceChar;
            set => this.blankSpaceChar = value;
        }

        /// <summary>
        /// Make full independed copy of <see cref="TgaComment"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaComment"/></returns>
        public TgaComment Clone() => new TgaComment(this.origString, this.blankSpaceChar);

        /// <summary>
        /// Make full independed copy of <see cref="TgaComment"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaComment"/></returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaComment) ? this.Equals((TgaComment)obj) : false);

        public bool Equals(TgaComment item) => (this.origString == item.origString && this.blankSpaceChar == item.blankSpaceChar);

        public static bool operator ==(TgaComment item1, TgaComment item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaComment item1, TgaComment item2) => !(item1 == item2);

        public override int GetHashCode() => this.origString.GetHashCode() ^ this.blankSpaceChar.GetHashCode();

        /// <summary>
        /// Get ASCII-Like string with string-terminators, example: "Line1 \0\0 Line2 \0\0\0".
        /// </summary>
        /// <returns>String with replaced string-terminators to "\0".</returns>
        public override string ToString() => Encoding.ASCII.GetString(this.ToBytes()).Replace("\0", @"\0");

        /// <summary>
        /// Get ASCII-Like string to first string-terminator, example:
        /// "Some string \0 Some Data \0" - > "Some string".
        /// </summary>
        /// <returns>String to first string-terminator.</returns>
        public string GetString() {
            var Str = Encoding.ASCII.GetString(this.ToBytes());
            for(var i = 1; i < 4; i++)
                Str = Str.Insert((StrNLen + 1) * i + i - 1, "\n");
            return Str.Replace("\0", string.Empty).TrimEnd(new char[] { '\n' });
        }

        /// <summary>
        /// Convert <see cref="TgaComment"/> to byte array.
        /// </summary>
        /// <returns>Byte array, every byte is ASCII symbol.</returns>
        public byte[] ToBytes() => ToBytes(this.origString, this.blankSpaceChar);

        /// <summary>
        /// Convert <see cref="TgaComment"/> to byte array.
        /// </summary>
        /// <param name="Str">Input string.</param>
        /// <param name="BlankSpaceChar">Char for filling blank space in string.</param>
        /// <returns>Byte array, every byte is ASCII symbol.</returns>
        public static byte[] ToBytes(string Str, char BlankSpaceChar = '\0') {
            var C = new char[81 * 4];

            for(var i = 0; i < C.Length; i++) {
                if((i + 82) % 81 == 0) {
                    C[i] = TgaString.DefaultEndingChar;
                } else {
                    var Index = i - i / 81;
                    C[i] = (Index < Str.Length ? Str[Index] : BlankSpaceChar);
                }
            }
            return Encoding.ASCII.GetBytes(C);
        }
    }

    public class TgaDateTime : ICloneable {
        private ushort month = 0;
        private ushort day = 0;
        private ushort year = 0;
        private ushort hour = 0;
        private ushort minute = 0;
        private ushort second = 0;

        /// <summary>
        /// Make empty <see cref="TgaDateTime"/>.
        /// </summary>
        public TgaDateTime() {
        }

        /// <summary>
        /// Make <see cref="TgaDateTime"/> from <see cref="DateTime"/>.
        /// </summary>
        /// <param name="DateAndTime">Some <see cref="DateTime"/> variable.</param>
        public TgaDateTime(DateTime DateAndTime) {
            this.month = (ushort)DateAndTime.Month;
            this.day = (ushort)DateAndTime.Day;
            this.year = (ushort)DateAndTime.Year;
            this.hour = (ushort)DateAndTime.Hour;
            this.minute = (ushort)DateAndTime.Minute;
            this.second = (ushort)DateAndTime.Second;
        }

        /// <summary>
        /// Make <see cref="TgaDateTime"/> from ushort values.
        /// </summary>
        /// <param name="Month">Month (1 - 12).</param>
        /// <param name="Day">Day (1 - 31).</param>
        /// <param name="Year">Year (4 digit, ie. 1989).</param>
        /// <param name="Hour">Hour (0 - 23).</param>
        /// <param name="Minute">Minute (0 - 59).</param>
        /// <param name="Second">Second (0 - 59).</param>
        public TgaDateTime(ushort Month, ushort Day, ushort Year, ushort Hour, ushort Minute, ushort Second) {
            this.month = Month;
            this.day = Day;
            this.year = Year;
            this.hour = Hour;
            this.minute = Minute;
            this.second = Second;
        }

        /// <summary>
        /// Make <see cref="TgaDateTime"/> from bytes.
        /// </summary>
        /// <param name="Bytes">Array of bytes(byte[12]).</param>
        public TgaDateTime(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            else if(Bytes.Length != Size)
                throw new ArgumentOutOfRangeException(nameof(Bytes) + " must be equal " + Size + "!");

            this.month = BitConverter.ToUInt16(Bytes, 0);
            this.day = BitConverter.ToUInt16(Bytes, 2);
            this.year = BitConverter.ToUInt16(Bytes, 4);
            this.hour = BitConverter.ToUInt16(Bytes, 6);
            this.minute = BitConverter.ToUInt16(Bytes, 8);
            this.second = BitConverter.ToUInt16(Bytes, 10);
        }

        /// <summary>
        /// Gets or Sets month (1 - 12).
        /// </summary>
        public ushort Month {
            get => this.month;
            set => this.month = value;
        }

        /// <summary>
        /// Gets or Sets day (1 - 31).
        /// </summary>
        public ushort Day {
            get => this.day;
            set => this.day = value;
        }

        /// <summary>
        /// Gets or Sets year (4 digit, ie. 1989).
        /// </summary>
        public ushort Year {
            get => this.year;
            set => this.year = value;
        }

        /// <summary>
        /// Gets or Sets hour (0 - 23).
        /// </summary>
        public ushort Hour {
            get => this.hour;
            set => this.hour = value;
        }

        /// <summary>
        /// Gets or Sets minute (0 - 59).
        /// </summary>
        public ushort Minute {
            get => this.minute;
            set => this.minute = value;
        }

        /// <summary>
        /// Gets or Sets second (0 - 59).
        /// </summary>
        public ushort Second {
            get => this.second;
            set => this.second = value;
        }

        /// <summary>
        /// Gets TGA Field size in bytes.
        /// </summary>
        public const int Size = 12;

        /// <summary>
        /// Make full independed copy of <see cref="TgaDateTime"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaDateTime"/></returns>
        public TgaDateTime Clone() => new TgaDateTime(this.month, this.day, this.year, this.hour, this.minute, this.second);

        /// <summary>
        /// Make full independed copy of <see cref="TgaDateTime"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaDateTime"/></returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaDateTime) ? this.Equals((TgaDateTime)obj) : false);

        public bool Equals(TgaDateTime item) => (
                this.month == item.month &&
                this.day == item.day &&
                this.year == item.year &&
                this.hour == item.hour &&
                this.minute == item.minute &&
                this.second == item.second);

        public static bool operator ==(TgaDateTime item1, TgaDateTime item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaDateTime item1, TgaDateTime item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                var hash = 17;
                hash = hash * 23 + (this.month << 16 | this.hour).GetHashCode();
                hash = hash * 23 + (this.day << 16 | this.minute).GetHashCode();
                hash = hash * 23 + (this.year << 16 | this.second).GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Gets <see cref="TgaDateTime"/> like string.
        /// </summary>
        /// <returns>String in "1990.01.23 1:02:03" format.</returns>
        public override string ToString() => string.Format("{0:D4}.{1:D2}.{2:D2} {3}:{4:D2}:{5:D2}", this.year, this.month, this.day, this.hour, this.minute, this.second);

        /// <summary>
        /// Convert <see cref="TgaDateTime"/> to byte array.
        /// </summary>
        /// <returns>Byte array with length = 12.</returns>
        public byte[] ToBytes() => BitConverterExt.ToBytes(this.month, this.day, this.year, this.hour, this.minute, this.second);

        /// <summary>
        /// Gets <see cref="TgaDateTime"/> like <see cref="DateTime"/>.
        /// </summary>
        /// <returns><see cref="DateTime"/> value of <see cref="TgaDateTime"/>.</returns>
        public DateTime ToDateTime() => new DateTime(this.year, this.month, this.day, this.hour, this.minute, this.second);
    }

    public class TgaDevEntry : ICloneable {
        // Directory
        private ushort fieldTag = 0;
        private uint fieldFileOffset = 0;

        // Field
        private byte[] data = null;

        /// <summary>
        /// Make empty <see cref="TgaDevEntry"/>.
        /// </summary>
        public TgaDevEntry() {
        }

        /// <summary>
        /// Make <see cref="TgaDevEntry"/> from other <see cref="TgaDevEntry"/>.
        /// </summary>
        /// <param name="Entry">Some <see cref="TgaDevEntry"/> variable.</param>
        public TgaDevEntry(TgaDevEntry Entry) {
            if(Entry == null)
                throw new ArgumentNullException();

            this.fieldTag = Entry.fieldTag;
            this.fieldFileOffset = Entry.fieldFileOffset;
            this.data = BitConverterExt.ToBytes(Entry.data);
        }

        /// <summary>
        /// Make <see cref="TgaDevEntry"/> from <see cref="Tag"/>, <see cref="Offset"/> and <see cref="FieldSize"/>.
        /// </summary>
        /// <param name="Tag">TAG ID (0 - 65535). See <see cref="Tag"/>.</param>
        /// <param name="Offset">TAG file offset in bytes. See <see cref="Offset"/>.</param>
        /// <param name="Data">This is DevEntry Field Data. See <see cref="Data"/>.</param>
        public TgaDevEntry(ushort Tag, uint Offset, byte[] Data = null) {
            this.fieldTag = Tag;
            this.fieldFileOffset = Offset;
            this.data = Data;
        }

        /// <summary>
        /// Make <see cref="TgaDevEntry"/> from bytes.
        /// </summary>
        /// <param name="Bytes">Array of bytes(byte[6] or bigger, if <see cref="Data"/> exist).</param>
        public TgaDevEntry(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            else if(Bytes.Length < 6)
                throw new ArgumentOutOfRangeException(nameof(Bytes) + " must be >= 6!");

            this.fieldTag = BitConverter.ToUInt16(Bytes, 0);
            this.fieldFileOffset = BitConverter.ToUInt32(Bytes, 2);

            if(Bytes.Length > 6)
                this.data = BitConverterExt.GetElements(Bytes, 6, Bytes.Length - 6);
        }

        /// <summary>
        /// Each TAG is a value in the range of 0 to 65535. Values from 0 - 32767 are available for developer use,
        /// while values from 32768 - 65535 are reserved for Truevision.
        /// </summary>
        public ushort Tag {
            get => this.fieldTag;
            set => this.fieldTag = value;
        }

        /// <summary>
        /// This OFFSET is a number of bytes from the beginning of the file to the start of the field
        /// referenced by the tag.
        /// </summary>
        public uint Offset {
            get => this.fieldFileOffset;
            set => this.fieldFileOffset = value;
        }

        /// <summary>
        /// Field DATA.
        /// Although the size and format of the actual Developer Area fields are totally up to the developer,
        /// please define your formats to address future considerations you might have concerning your fields.
        /// This means that if you anticipate changing a field, build flexibility into the format to make these
        /// changes easy on other developers.Major changes to an existing TAG’s definition should never happen.
        /// </summary>
        public byte[] Data {
            get => this.data;
            set => this.data = value;
        }

        /// <summary>
        /// The FIELD SIZE is a number of bytes in the field. Same like: <see cref="Data.Length"/>,
        /// if <see cref="Data"/> is null, return -1.
        /// </summary>
        public int FieldSize {
            get {
                if(this.Data == null)
                    return -1;

                return this.Data.Length;
            }
        }

        /// <summary>
        /// Gets TGA <see cref="TgaDevEntry"/> size in bytes (Always constant and equal 10!).
        /// It is not <see cref="FieldSize"/>! It is just size of entry sizeof(ushort + uint + uint).
        /// </summary>
        public const int Size = 10;

        /// <summary>
        /// Make full independed copy of <see cref="TgaDevEntry"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaDevEntry"/></returns>
        public TgaDevEntry Clone() => new TgaDevEntry(this);

        /// <summary>
        /// Make full independed copy of <see cref="TgaDevEntry"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaDevEntry"/></returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaDevEntry) ? this.Equals((TgaDevEntry)obj) : false);

        public bool Equals(TgaDevEntry item) => (this.fieldTag == item.fieldTag &&
                this.fieldFileOffset == item.fieldFileOffset &&
                BitConverterExt.IsArraysEqual(this.data, item.data));

        public static bool operator ==(TgaDevEntry item1, TgaDevEntry item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaDevEntry item1, TgaDevEntry item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                var hash = 17;
                hash = hash * 23 + this.fieldTag.GetHashCode();
                hash = hash * 23 + this.fieldFileOffset.GetHashCode();

                if(this.data != null) {
                    for(var i = 0; i < this.data.Length; i++)
                        hash = hash * 23 + this.data[i].GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Gets <see cref="TgaDevEntry"/> like string.
        /// </summary>
        /// <returns>String in "Tag={0}, Offset={1}, FieldSize={2}" format.</returns>
        public override string ToString() => string.Format("{0}={1}, {1}={2}, {3}={4}", nameof(this.Tag), this.fieldTag,
                nameof(this.Offset), this.fieldFileOffset, nameof(this.FieldSize), this.FieldSize);

        /// <summary>
        /// Convert <see cref="TgaDevEntry"/> to byte array. (Not include <see cref="Data"/>!).
        /// </summary>
        /// <returns>Byte array with length = 10.</returns>
        public byte[] ToBytes() => BitConverterExt.ToBytes(this.fieldTag, this.fieldFileOffset, (this.data == null ? 0 : this.data.Length));
    } //Not full ToBytes()

    public class TgaFraction : ICloneable {
        private ushort numerator = 0;
        private ushort denominator = 0;

        /// <summary>
        /// Make <see cref="TgaFraction"/> from <see cref="Numerator"/> and <see cref="Denominator"/>.
        /// </summary>
        /// <param name="Numerator">Numerator value.</param>
        /// <param name="Denominator">Denominator value.</param>
        public TgaFraction(ushort Numerator = 0, ushort Denominator = 0) {
            this.numerator = Numerator;
            this.denominator = Denominator;
        }

        /// <summary>
        /// Make <see cref="TgaFraction"/> from bytes.
        /// </summary>
        /// <param name="Bytes">Array of bytes(byte[4]).</param>
        public TgaFraction(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            if(Bytes.Length != Size)
                throw new ArgumentOutOfRangeException(nameof(Bytes.Length) + " must be equal " + Size + "!");

            this.numerator = BitConverter.ToUInt16(Bytes, 0);
            this.denominator = BitConverter.ToUInt16(Bytes, 2);
        }

        /// <summary>
        /// Gets or sets numerator value.
        /// </summary>
        public ushort Numerator {
            get => this.numerator;
            set => this.numerator = value;
        }

        /// <summary>
        /// Gets or sets denominator value.
        /// </summary>
        public ushort Denominator {
            get => this.denominator;
            set => this.denominator = value;
        }

        /// <summary>
        /// Get aspect ratio = <see cref="Numerator"/> / <see cref="Denominator"/>.
        /// </summary>
        public float AspectRatio {
            get {
                if(this.numerator == this.denominator)
                    return 1f;

                return this.numerator / (float)this.denominator;
            }
        }

        /// <summary>
        /// Gets Empty <see cref="TgaFraction"/>, all values are 0.
        /// </summary>
        public static readonly TgaFraction Empty = new TgaFraction();

        /// <summary>
        /// Gets One <see cref="TgaFraction"/>, all values are 1 (ones, 1 / 1 = 1).
        /// </summary>
        public static readonly TgaFraction One = new TgaFraction(1, 1);

        /// <summary>
        /// Gets TGA Field size in bytes.
        /// </summary>
        public const int Size = 4;

        /// <summary>
        /// Make full independed copy of <see cref="TgaFraction"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaFraction"/></returns>
        public TgaFraction Clone() => new TgaFraction(this.numerator, this.denominator);

        /// <summary>
        /// Make full independed copy of <see cref="TgaFraction"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaFraction"/></returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaFraction) ? this.Equals((TgaFraction)obj) : false);

        public bool Equals(TgaFraction item) => (this.numerator == item.numerator && this.denominator == item.denominator);

        public static bool operator ==(TgaFraction item1, TgaFraction item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaFraction item1, TgaFraction item2) => !(item1 == item2);

        public override int GetHashCode() => (this.numerator << 16 | this.denominator).GetHashCode();

        /// <summary>
        /// Gets <see cref="TgaFraction"/> like string.
        /// </summary>
        /// <returns>String in "Numerator=1, Denominator=2" format.</returns>
        public override string ToString() => string.Format("{0}={1}, {2}={3}", nameof(this.Numerator), this.numerator,
                nameof(this.Denominator), this.denominator);

        /// <summary>
        /// Convert <see cref="TgaFraction"/> to byte array.
        /// </summary>
        /// <returns>Byte array with length = 4.</returns>
        public byte[] ToBytes() => BitConverterExt.ToBytes(this.numerator, this.denominator);
    }

    /// <summary>
    /// Contains image origin bits and alpha channel bits(or number of overlay bits)
    /// </summary>
    public class TgaImageDescriptor : ICloneable {
        private TgaImgOrigin imageOrigin = 0; //bits 5-4
        private byte alphaChannelBits = 0; //bits 3-0

        /// <summary>
        /// Make empty <see cref="TgaImageDescriptor"/>.
        /// </summary>
        public TgaImageDescriptor() {
        }

        /// <summary>
        /// Make <see cref="TgaImageDescriptor"/> from bytes.
        /// </summary>
        /// <param name="b">ImageDescriptor byte with reserved 7-6 bits, bits 5-4 used for
        /// <see cref="ImageOrigin"/>, 3-0 used as alpha channel bits or number of overlay bits.</param>
        public TgaImageDescriptor(byte b) {
            this.imageOrigin = (TgaImgOrigin)((b & 0x30) >> 4);
            this.alphaChannelBits = (byte)(b & 0x0F);
        }

        /// <summary>
        /// Gets or Sets Image Origin bits (select from enum only, don'n use 5-4 bits!).
        /// </summary>
        public TgaImgOrigin ImageOrigin {
            get => this.imageOrigin;
            set => this.imageOrigin = value;
        }

        /// <summary>
        /// Gets or Sets alpha channel bits or number of overlay bits.
        /// </summary>
        public byte AlphaChannelBits {
            get => this.alphaChannelBits;
            set => this.alphaChannelBits = value;
        }

        /// <summary>
        /// Gets TGA Field size in bytes.
        /// </summary>
        public const int Size = 1;

        /// <summary>
        /// Make full copy of <see cref="TgaImageDescriptor"/>.
        /// </summary>
        /// <returns>Full independent copy of <see cref="TgaImageDescriptor"/>.</returns>
        public TgaImageDescriptor Clone() => new TgaImageDescriptor(this.ToByte());

        /// <summary>
        /// Make full copy of <see cref="TgaImageDescriptor"/>.
        /// </summary>
        /// <returns>Full independent copy of <see cref="TgaImageDescriptor"/>.</returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaImageDescriptor) ? this.Equals((TgaImageDescriptor)obj) : false);

        public bool Equals(TgaImageDescriptor item) => (this.imageOrigin == item.imageOrigin && this.alphaChannelBits == item.alphaChannelBits);

        public static bool operator ==(TgaImageDescriptor item1, TgaImageDescriptor item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaImageDescriptor item1, TgaImageDescriptor item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                return ((int)this.ImageOrigin << 4 | this.alphaChannelBits).GetHashCode();
            }
        }

        public override string ToString() => string.Format("{0}={1}, {2}={3}, ImageDescriptor_AsByte={4}", nameof(this.ImageOrigin),
                this.imageOrigin, nameof(this.AlphaChannelBits), this.alphaChannelBits, this.ToByte());

        /// <summary>
        /// Gets ImageDescriptor byte.
        /// </summary>
        /// <returns>ImageDescriptor byte with reserved 7-6 bits, bits 5-4 used for imageOrigin,
        /// 3-0 used as alpha channel bits or number of overlay bits.</returns>
        public byte ToByte() => (byte)(((int)this.imageOrigin << 4) | this.alphaChannelBits);
    }

    /// <summary>
    /// Image Specification - Field 5 (10 bytes):
    /// <para>This field and its sub-fields describe the image screen location, size and pixel depth.
    /// These information is always written to the file.</para>
    /// </summary>
    public class TgaImageSpec : ICloneable {
        private ushort x_Origin = 0;
        private ushort y_Origin = 0;
        private ushort imageWidth = 0;
        private ushort imageHeight = 0;
        private TgaPixelDepth pixelDepth = TgaPixelDepth.Other;
        private TgaImageDescriptor imageDescriptor = new TgaImageDescriptor();

        public TgaImageSpec() {
        }

        /// <summary>
        /// Make ImageSpec from values.
        /// </summary>
        /// <param name="X_Origin">These specify the absolute horizontal coordinate for the lower
        /// left corner of the image as it is positioned on a display device having an origin at
        /// the lower left of the screen(e.g., the TARGA series).</param>
        /// <param name="Y_Origin">These specify the absolute vertical coordinate for the lower
        /// left corner of the image as it is positioned on a display device having an origin at
        /// the lower left of the screen(e.g., the TARGA series).</param>
        /// <param name="ImageWidth">This field specifies the width of the image in pixels.</param>
        /// <param name="ImageHeight">This field specifies the height of the image in pixels.</param>
        /// <param name="PixelDepth">This field indicates the number of bits per pixel. This number
        /// includes the Attribute or Alpha channel bits. Common values are 8, 16, 24 and 32 but
        /// other pixel depths could be used.</param>
        /// <param name="ImageDescriptor">Contains image origin bits and alpha channel bits
        /// (or number of overlay bits).</param>
        public TgaImageSpec(ushort X_Origin, ushort Y_Origin, ushort ImageWidth, ushort ImageHeight,
            TgaPixelDepth PixelDepth, TgaImageDescriptor ImageDescriptor) {
            this.x_Origin = X_Origin;
            this.y_Origin = Y_Origin;
            this.imageWidth = ImageWidth;
            this.imageHeight = ImageHeight;
            this.pixelDepth = PixelDepth;
            this.imageDescriptor = ImageDescriptor;
        }

        /// <summary>
        /// Make ImageSpec from bytes.
        /// </summary>
        /// <param name="Bytes">Array of bytes(byte[10]).</param>
        public TgaImageSpec(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            if(Bytes.Length != Size)
                throw new ArgumentOutOfRangeException(nameof(Bytes.Length) + " must be equal " + Size + "!");

            this.x_Origin = BitConverter.ToUInt16(Bytes, 0);
            this.y_Origin = BitConverter.ToUInt16(Bytes, 2);
            this.imageWidth = BitConverter.ToUInt16(Bytes, 4);
            this.imageHeight = BitConverter.ToUInt16(Bytes, 6);
            this.pixelDepth = (TgaPixelDepth)Bytes[8];
            this.imageDescriptor = new TgaImageDescriptor(Bytes[9]);
        }

        /// <summary>
        /// These specify the absolute horizontal coordinate for the lower left corner of the image
        /// as it is positioned on a display device having an origin at the lower left of the
        /// screen(e.g., the TARGA series).
        /// </summary>
        public ushort X_Origin {
            get => this.x_Origin;
            set => this.x_Origin = value;
        }

        /// <summary>
        /// These specify the absolute vertical coordinate for the lower left corner of the image
        /// as it is positioned on a display device having an origin at the lower left of the
        /// screen(e.g., the TARGA series).
        /// </summary>
        public ushort Y_Origin {
            get => this.y_Origin;
            set => this.y_Origin = value;
        }

        /// <summary>
        /// This field specifies the width of the image in pixels.
        /// </summary>
        public ushort ImageWidth {
            get => this.imageWidth;
            set => this.imageWidth = value;
        }

        /// <summary>
        /// This field specifies the height of the image in pixels.
        /// </summary>
        public ushort ImageHeight {
            get => this.imageHeight;
            set => this.imageHeight = value;
        }

        /// <summary>
        /// This field indicates the number of bits per pixel. This number includes the Attribute or
        /// Alpha channel bits. Common values are 8, 16, 24 and 32 but other pixel depths could be used.
        /// </summary>
        public TgaPixelDepth PixelDepth {
            get => this.pixelDepth;
            set => this.pixelDepth = value;
        }

        /// <summary>
        /// Contains image origin bits and alpha channel bits(or number of overlay bits).
        /// </summary>
        public TgaImageDescriptor ImageDescriptor {
            get => this.imageDescriptor;
            set => this.imageDescriptor = value;
        }

        /// <summary>
        /// Gets TGA Field size in bytes.
        /// </summary>
        public const int Size = 10;

        /// <summary>
        /// Make full copy of <see cref="TgaImageDescriptor"/>.
        /// </summary>
        /// <returns></returns>
        public TgaImageSpec Clone() => new TgaImageSpec(this.ToBytes());

        /// <summary>
        /// Make full copy of <see cref="TgaImageDescriptor"/>.
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaImageSpec) ? this.Equals((TgaImageSpec)obj) : false);

        public bool Equals(TgaImageSpec item) => (
                this.x_Origin == item.x_Origin &&
                this.y_Origin == item.y_Origin &&
                this.imageWidth == item.imageWidth &&
                this.imageHeight == item.imageHeight &&
                this.pixelDepth == item.pixelDepth &&
                this.imageDescriptor == item.imageDescriptor);

        public static bool operator ==(TgaImageSpec item1, TgaImageSpec item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaImageSpec item1, TgaImageSpec item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                var hash = 17;
                hash = hash * 23 + this.x_Origin.GetHashCode();
                hash = hash * 23 + this.y_Origin.GetHashCode();
                hash = hash * 23 + this.imageWidth.GetHashCode();
                hash = hash * 23 + this.imageHeight.GetHashCode();
                hash = hash * 23 + this.pixelDepth.GetHashCode();

                if(this.imageDescriptor != null)
                    hash = hash * 23 + this.imageDescriptor.GetHashCode();

                return hash;
            }
        }

        public override string ToString() => string.Format("{0}={1}, {2}={3}, {4}={5}, {6}={7}, {8}={9}, {10}={11}",
                nameof(this.X_Origin), this.x_Origin,
                nameof(this.Y_Origin), this.y_Origin,
                nameof(this.ImageWidth), this.imageWidth,
                nameof(this.ImageHeight), this.imageHeight,
                nameof(this.PixelDepth), this.pixelDepth,
                nameof(this.ImageDescriptor), this.imageDescriptor);

        /// <summary>
        /// Convert <see cref="TgaImageSpec"/> to byte array.
        /// </summary>
        /// <returns>Byte array with length = 10.</returns>
        public byte[] ToBytes() => BitConverterExt.ToBytes(this.x_Origin, this.y_Origin, this.imageWidth, this.imageHeight,
                (byte)this.pixelDepth, (this.imageDescriptor == null ? byte.MinValue : this.imageDescriptor.ToByte()));
    }

    /// <summary>
    /// Postage Stamp Image (MaxSize 64x64, uncompressed, PixelDepth like in full image).
    /// </summary>
    public class TgaPostageStampImage : ICloneable {
        private byte width = 0;
        private byte height = 0;
        private byte[] data = null;

        public TgaPostageStampImage() {
        }

        /// <summary>
        /// Make <see cref="TgaPostageStampImage"/> from bytes array.
        /// </summary>
        /// <param name="Bytes">Bytes array, first 2 bytes are <see cref="Width"/> and <see cref="Height"/>,
        /// next bytes - image data.</param>
        public TgaPostageStampImage(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            if(Bytes.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(Bytes.Length) + " must be >= " + 2 + "!");

            this.width = Bytes[0];
            this.height = Bytes[1];

            if(Bytes.Length > 2)
                this.data = BitConverterExt.GetElements(Bytes, 2, Bytes.Length - 2);
        }

        /// <summary>
        /// Make <see cref="TgaPostageStampImage"/> from bytes and size.
        /// </summary>
        /// <param name="Width">Image Width.</param>
        /// <param name="Height">Image Height.</param>
        /// <param name="Bytes">Postage Stamp Image Data.</param>
        public TgaPostageStampImage(byte Width, byte Height, byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");

            this.width = Width;
            this.height = Height;
            this.data = Bytes;
        }

        /// <summary>
        /// Postage Stamp Image Data
        /// </summary>
        public byte[] Data {
            get => this.data;
            set => this.data = value;
        }

        /// <summary>
        /// Postage Stamp Image Width (maximum = 64).
        /// </summary>
        public byte Width {
            get => this.width;
            set => this.width = value;
        }

        /// <summary>
        /// Postage Stamp Image Height (maximum = 64).
        /// </summary>
        public byte Height {
            get => this.height;
            set => this.height = value;
        }

        /// <summary>
        /// Make full copy of <see cref="TgaPostageStampImage"/>.
        /// </summary>
        /// <returns>Full independent copy of <see cref="TgaPostageStampImage"/>.</returns>
        public TgaPostageStampImage Clone() => new TgaPostageStampImage(this.width, this.height, BitConverterExt.ToBytes(this.data));

        /// <summary>
        /// Make full copy of <see cref="TgaPostageStampImage"/>.
        /// </summary>
        /// <returns>Full independent copy of <see cref="TgaPostageStampImage"/>.</returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaPostageStampImage) ? this.Equals((TgaPostageStampImage)obj) : false);

        public bool Equals(TgaPostageStampImage item) => this.width == item.width && this.height == item.height && BitConverterExt.IsArraysEqual(this.data, item.data);

        public static bool operator ==(TgaPostageStampImage item1, TgaPostageStampImage item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaPostageStampImage item1, TgaPostageStampImage item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                var hash = 27;
                hash = (13 * hash) + this.width.GetHashCode();
                hash = (13 * hash) + this.height.GetHashCode();
                if(this.data != null) {
                    for(var i = 0; i < this.data.Length; i++)
                        hash = (13 * hash) + this.data[i].GetHashCode();
                }

                return hash;
            }
        }

        public override string ToString() => string.Format("{0}={1}, {2}={3}, DataLength={4}",
                nameof(this.Width), this.width, nameof(this.Height), this.height, (this.data == null ? -1 : this.data.Length));

        /// <summary>
        /// Convert <see cref="TgaPostageStampImage"/> to byte array.
        /// </summary>
        /// <returns>Byte array.</returns>
        public byte[] ToBytes() => BitConverterExt.ToBytes(this.width, this.height, this.data);
    }

    public class TgaSoftVersion : ICloneable {
        private ushort versionNumber = 0;
        private char versionLetter = ' ';

        /// <summary>
        /// Gets Empty <see cref="TgaSoftVersion"/>, <see cref="VersionLetter"/> = ' ' (space).
        /// </summary>
        public TgaSoftVersion() {
        }

        /// <summary>
        /// Make <see cref="TgaSoftVersion"/> from string.
        /// </summary>
        /// <param name="Str">Input string, example: "123d".</param>
        public TgaSoftVersion(string Str) {
            if(Str == null)
                throw new ArgumentNullException();
            if(Str.Length < 3 || Str.Length > 4)
                throw new ArgumentOutOfRangeException(nameof(Str.Length) + " must be equal 3 or 4!");

            var Res = ushort.TryParse(Str.Substring(0, 3), out this.versionNumber);
            if(Res && Str.Length == 4)
                this.versionLetter = Str[3];
        }

        /// <summary>
        /// Make <see cref="TgaSoftVersion"/> from bytes.
        /// </summary>
        /// <param name="Bytes">Bytes array (byte[3]).</param>
        public TgaSoftVersion(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            if(Bytes.Length != Size)
                throw new ArgumentOutOfRangeException(nameof(Bytes.Length) + " must be equal " + Size + "!");

            this.versionNumber = BitConverter.ToUInt16(Bytes, 0);
            this.versionLetter = Encoding.ASCII.GetString(Bytes, 2, 1)[0];
        }

        public TgaSoftVersion(ushort VersionNumber, char VersionLetter = ' ') {
            this.versionNumber = VersionNumber;
            this.versionLetter = VersionLetter;
        }

        public ushort VersionNumber {
            get => this.versionNumber;
            set => this.versionNumber = value;
        }

        public char VersionLetter {
            get => this.versionLetter;
            set => this.versionLetter = value;
        }

        /// <summary>
        /// Gets TGA Field size in bytes.
        /// </summary>
        public const int Size = 3;

        /// <summary>
        /// Make full copy of <see cref="TgaSoftVersion"/>.
        /// </summary>
        /// <returns></returns>
        public TgaSoftVersion Clone() => new TgaSoftVersion(this.versionNumber, this.versionLetter);

        /// <summary>
        /// Make full copy of <see cref="TgaSoftVersion"/>.
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaSoftVersion) ? this.Equals((TgaSoftVersion)obj) : false);

        public bool Equals(TgaSoftVersion item) => (this.versionNumber == item.versionNumber && this.versionLetter == item.versionLetter);

        public static bool operator ==(TgaSoftVersion item1, TgaSoftVersion item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaSoftVersion item1, TgaSoftVersion item2) => !(item1 == item2);

        public override int GetHashCode() => this.versionNumber.GetHashCode() ^ this.versionLetter.GetHashCode();

        public override string ToString() => (this.versionNumber.ToString("000") + this.versionLetter).TrimEnd(new char[] { ' ', '\0' });

        /// <summary>
        /// Convert <see cref="TgaSoftVersion"/> to byte array.
        /// </summary>
        /// <returns>Byte array, <see cref="VersionNumber"/> (2 bytes) and
        /// <see cref="VersionLetter"/> (ASCII symbol).</returns>
        public byte[] ToBytes() => ToBytes(this.versionNumber, this.versionLetter);

        /// <summary>
        /// Convert <see cref="TgaSoftVersion"/> to byte array.
        /// </summary>
        /// <param name="VersionNumber">Set 123 for 1.23 version.</param>
        /// <param name="VersionLetter">Version letter, example: for 'a' - "1.23a".</param>
        /// <returns>Byte array, <see cref="VersionNumber"/> (2 bytes) and <see cref="VersionLetter"/> (ASCII symbol).</returns>
        public static byte[] ToBytes(ushort VersionNumber, char VersionLetter = ' ') => BitConverterExt.ToBytes(VersionNumber, Encoding.ASCII.GetBytes(VersionLetter.ToString()));
    }

    /// <summary>
    /// Use it for working with ASCII strings in TGA files.
    /// </summary>
    public class TgaString : ICloneable {
        public const string XFileSignatuteConst = "TRUEVISION-XFILE";
        public const string DotSymbolConst = ".";
        private string origString = string.Empty;
        private int length = 0;
        private char blankSpaceChar = DefaultBlankSpaceChar;
        private bool useEnding = false;

        public TgaString(bool UseEnding = false) => this.useEnding = UseEnding;

        public TgaString(byte[] Bytes, bool UseEnding = false) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");

            this.length = Bytes.Length;
            this.useEnding = UseEnding;
            var s = Encoding.ASCII.GetString(Bytes, 0, Bytes.Length - (this.useEnding ? 1 : 0));

            if(s.Length > 0) {
                switch(s[s.Length - 1]) {
                case '\0':
                case ' ':
                    this.blankSpaceChar = s[s.Length - 1];
                    this.origString = s.TrimEnd(new char[] { s[s.Length - 1] });
                    break;
                default:
                    this.origString = s;
                    break;
                }
            }
        }

        public TgaString(int Length, bool UseEnding = false) {
            this.length = Length;
            this.useEnding = UseEnding;
        }

        public TgaString(string Str, int Length, bool UseEnding = false, char BlankSpaceChar = '\0') {
            if(Str == null)
                throw new ArgumentNullException(nameof(Str) + " = null!");

            this.origString = Str;
            this.length = Length;
            this.blankSpaceChar = BlankSpaceChar;
            this.useEnding = UseEnding;
        }

        public string OriginalString {
            get => this.origString;
            set => this.origString = value;
        }

        public int Length {
            get => this.length;
            set => this.length = value;
        }

        public char BlankSpaceChar {
            get => this.blankSpaceChar;
            set => this.blankSpaceChar = value;
        }

        public bool UseEndingChar {
            get => this.useEnding;
            set => this.useEnding = value;
        }

        /// <summary>
        /// Gets ending char, default '\0'.
        /// </summary>
        public static readonly char DefaultEndingChar = '\0';

        /// <summary>
        /// Gets blank space char, value = '\0'.
        /// </summary>
        public static readonly char DefaultBlankSpaceChar = '\0';

        /// <summary>
        /// Gets Empty <see cref="TgaString"/>.
        /// </summary>
        public static readonly TgaString Empty = new TgaString();

        /// <summary>
        /// Gets <see cref="TgaString"/> with <see cref="DefaultEndingChar"/> = '\0' and <see cref="UseEndingChar"/> = true.
        /// </summary>
        public static readonly TgaString ZeroTerminator = new TgaString(true);

        /// <summary>
        /// Gets "." <see cref="TgaString"/> with dot (period) symbol.
        /// </summary>
        public static readonly TgaString DotSymbol = new TgaString(DotSymbolConst, DotSymbolConst.Length);

        /// <summary>
        /// Gets "TRUEVISION-XFILE" <see cref="TgaString"/> (TGA File Format Version 2.0 signatute).
        /// </summary>
        public static readonly TgaString XFileSignatute = new TgaString(XFileSignatuteConst, XFileSignatuteConst.Length);

        /// <summary>
        /// Make full independed copy of <see cref="TgaString"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaString"/></returns>
        public TgaString Clone() => new TgaString(this.origString, this.length, this.useEnding, this.blankSpaceChar);

        /// <summary>
        /// Make full independed copy of <see cref="TgaString"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaString"/></returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaString) ? this.Equals((TgaString)obj) : false);

        public bool Equals(TgaString item) => (
                this.origString == item.origString &&
                this.length == item.length &&
                this.blankSpaceChar == item.blankSpaceChar &&
                this.useEnding == item.useEnding);

        public static bool operator ==(TgaString item1, TgaString item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaString item1, TgaString item2) => !(item1 == item2);

        public static TgaString operator +(TgaString item1, TgaString item2) {
            if(ReferenceEquals(item1, null) || ReferenceEquals(item2, null))
                throw new ArgumentNullException();

            return new TgaString(BitConverterExt.ToBytes(item1.ToBytes(), item2.ToBytes()));
        }

        public override int GetHashCode() {
            unchecked {
                var hash = 17;
                hash = hash * 23 + this.origString.GetHashCode();
                hash = hash * 23 + this.length.GetHashCode();
                hash = hash * 23 + this.blankSpaceChar.GetHashCode();
                hash = hash * 23 + this.useEnding.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Get ASCII-Like string with string-terminators, example: "Some string\0\0\0\0\0".
        /// </summary>
        /// <returns>String with replaced string-terminators to "\0".</returns>
        public override string ToString() => Encoding.ASCII.GetString(this.ToBytes()).Replace("\0", @"\0");

        /// <summary>
        /// Get ASCII-Like string to first string-terminator, example:
        /// "Some string \0 Some Data \0" - > "Some string".
        /// </summary>
        /// <returns>String to first string-terminator.</returns>
        public string GetString() {
            var Str = Encoding.ASCII.GetString(this.ToBytes());
            var EndIndex = Str.IndexOf('\0');
            if(EndIndex != -1)
                Str = Str.Substring(0, EndIndex);
            return Str;
        }

        /// <summary>
        /// Convert <see cref="TgaString"/> to byte array.
        /// </summary>
        /// <returns>Byte array, every byte is ASCII symbol.</returns>
        public byte[] ToBytes() => ToBytes(this.origString, this.length, this.useEnding, this.blankSpaceChar);

        /// <summary>
        /// Convert <see cref="TgaString"/> to byte array.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <param name="Length">Length of output ASCII string with Ending char (if used).</param>
        /// <param name="UseEnding">Add <see cref="EndingChr"/> to string or not?</param>
        /// <param name="BlankSpaceChar">Char for filling blank space in string. If this char is '-' (only for example!),
        /// for string "ABC" with <see cref="Length"/> = 7, with <see cref="UseEnding"/> = true,
        /// <see cref="DefaultEndingChar"/> is '\0', result string is "ABC---\0".</param>
        /// <returns>Byte array, every byte is ASCII symbol.</returns>
        public static byte[] ToBytes(string str, int Length, bool UseEnding = true, char BlankSpaceChar = '\0') {
            var C = new char[Math.Max(Length, (UseEnding ? 1 : 0))];

            for(var i = 0; i < C.Length; i++)
                C[i] = (i < str.Length ? str[i] : BlankSpaceChar);

            if(UseEnding)
                C[C.Length - 1] = DefaultEndingChar;

            return Encoding.ASCII.GetBytes(C);
        }
    }

    public class TgaTime : ICloneable {
        private ushort hours = 0;
        private ushort minutes = 0;
        private ushort seconds = 0;

        /// <summary>
        /// Make empty <see cref="TgaTime"/>.
        /// </summary>
        public TgaTime() {
        }

        /// <summary>
        /// Make <see cref="TgaTime"/> from <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="Time">Some <see cref="TimeSpan"/> variable.</param>
        public TgaTime(TimeSpan Time) {
            this.hours = (ushort)Time.TotalHours;
            this.minutes = (ushort)Time.Minutes;
            this.seconds = (ushort)Time.Seconds;
        }

        /// <summary>
        /// Make <see cref="TgaTime"/> from ushort values.
        /// </summary>
        /// <param name="Hours">Hour (0 - 65535).</param>
        /// <param name="Minutes">Minute (0 - 59).</param>
        /// <param name="Seconds">Second (0 - 59).</param>
        public TgaTime(ushort Hours, ushort Minutes, ushort Seconds) {
            this.hours = Hours;
            this.minutes = Minutes;
            this.seconds = Seconds;
        }

        /// <summary>
        /// Make <see cref="TgaTime"/> from bytes.
        /// </summary>
        /// <param name="Bytes">Array of bytes(byte[6]).</param>
        public TgaTime(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            else if(Bytes.Length != Size)
                throw new ArgumentOutOfRangeException(nameof(Bytes) + " must be equal " + Size + "!");

            this.hours = BitConverter.ToUInt16(Bytes, 0);
            this.minutes = BitConverter.ToUInt16(Bytes, 2);
            this.seconds = BitConverter.ToUInt16(Bytes, 4);
        }

        /// <summary>
        /// Gets or Sets hour (0 - 65535).
        /// </summary>
        public ushort Hours {
            get => this.hours;
            set => this.hours = value;
        }

        /// <summary>
        /// Gets or Sets minute (0 - 59).
        /// </summary>
        public ushort Minutes {
            get => this.minutes;
            set => this.minutes = value;
        }

        /// <summary>
        /// Gets or Sets second (0 - 59).
        /// </summary>
        public ushort Seconds {
            get => this.seconds;
            set => this.seconds = value;
        }

        /// <summary>
        /// Gets TGA Field size in bytes.
        /// </summary>
        public const int Size = 6;

        /// <summary>
        /// Make full independed copy of <see cref="TgaTime"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaTime"/></returns>
        public TgaTime Clone() => new TgaTime(this.hours, this.minutes, this.seconds);

        /// <summary>
        /// Make full independed copy of <see cref="TgaTime"/>.
        /// </summary>
        /// <returns>Copy of <see cref="TgaTime"/></returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaTime) ? this.Equals((TgaTime)obj) : false);

        public bool Equals(TgaTime item) => (this.hours == item.hours && this.minutes == item.minutes && this.seconds == item.seconds);

        public static bool operator ==(TgaTime item1, TgaTime item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaTime item1, TgaTime item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                var hash = 17;
                hash = hash * 23 + this.hours.GetHashCode();
                hash = hash * 23 + (this.minutes << 16 | this.seconds).GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Gets <see cref="TgaTime"/> like string.
        /// </summary>
        /// <returns>String in "H:M:S" format.</returns>
        public override string ToString() => string.Format("{0}:{1}:{2}", this.hours, this.minutes, this.seconds);

        /// <summary>
        /// Convert <see cref="TgaTime"/> to byte array.
        /// </summary>
        /// <returns>Byte array with length = 6.</returns>
        public byte[] ToBytes() => BitConverterExt.ToBytes(this.hours, this.minutes, this.seconds);

        /// <summary>
        /// Gets <see cref="TgaTime"/> like <see cref="TimeSpan"/>.
        /// </summary>
        /// <returns><see cref="TimeSpan"/> value of <see cref="TgaTime"/>.</returns>
        public TimeSpan ToTimeSpan() => new TimeSpan(this.hours, this.minutes, this.seconds);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// File Header Area (18 bytes)
    /// </summary>
    public class TgaHeader : ICloneable {
        private byte idLength = 0;
        private TgaColorMapType colorMapType = TgaColorMapType.NoColorMap;
        private TgaImageType imageType = TgaImageType.NoImageData;
        private TgaColorMapSpec colorMapSpec = new TgaColorMapSpec();
        private TgaImageSpec imageSpec = new TgaImageSpec();

        /// <summary>
        /// Make empty <see cref="TgaHeader"/>.
        /// </summary>
        public TgaHeader() {
        }

        /// <summary>
        /// Make <see cref="TgaHeader"/> from bytes.
        /// </summary>
        /// <param name="Bytes">Bytes array (byte[18]).</param>
        public TgaHeader(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            if(Bytes.Length != Size)
                throw new ArgumentOutOfRangeException(nameof(Bytes.Length) + " must be equal " + Size + "!");

            this.idLength = Bytes[0];
            this.colorMapType = (TgaColorMapType)Bytes[1];
            this.imageType = (TgaImageType)Bytes[2];
            this.colorMapSpec = new TgaColorMapSpec(BitConverterExt.GetElements(Bytes, 3, TgaColorMapSpec.Size));
            this.imageSpec = new TgaImageSpec(BitConverterExt.GetElements(Bytes, 8, TgaImageSpec.Size));
        }

        /// <summary>
        /// ID Length - Field 1 (1 byte):
        /// This field identifies the number of bytes contained in the <see cref="ImageID"/> Field.
        /// The maximum number of characters is 255. A value of zero indicates that no Image ID
        /// field is included with the image.
        /// </summary>
        public byte IDLength {
            get => this.idLength;
            set => this.idLength = value;
        }

        /// <summary>
        /// Color Map Type - Field 2 (1 byte):
        /// This field indicates the type of color map (if any) included with the image.
        /// There are currently 2 defined values for this field:
        /// <para>0 - indicates that no color-map data is included with this image;</para>
        /// <para>1 - indicates that a color-map is included with this image.</para>
        /// </summary>
        public TgaColorMapType ColorMapType {
            get => this.colorMapType;
            set => this.colorMapType = value;
        }

        /// <summary>
        /// Image Type - Field 3 (1 byte):
        /// <para>The TGA File Format can be used to store Pseudo-Color, True-Color and Direct-Color images
        /// of various pixel depths.</para>
        /// </summary>
        public TgaImageType ImageType {
            get => this.imageType;
            set => this.imageType = value;
        }

        /// <summary>
        /// Color Map Specification - Field 4 (5 bytes):
        /// <para>This field and its sub-fields describe the color map (if any) used for the image.
        /// If the Color Map Type field is set to zero, indicating that no color map exists, then
        /// these 5 bytes should be set to zero. These bytes always must be written to the file.</para>
        /// </summary>
        public TgaColorMapSpec ColorMapSpec {
            get => this.colorMapSpec;
            set => this.colorMapSpec = value;
        }

        /// <summary>
        /// Image Specification - Field 5 (10 bytes):
        /// <para>This field and its sub-fields describe the image screen location, size and pixel depth.
        /// These information is always written to the file.</para>
        /// </summary>
        public TgaImageSpec ImageSpec {
            get => this.imageSpec;
            set => this.imageSpec = value;
        }

        /// <summary>
        /// Gets TGA Header Section size in bytes.
        /// </summary>
        public const int Size = 18;

        /// <summary>
        /// Make full copy of <see cref="TgaHeader"/>.
        /// </summary>
        /// <returns>Full independent copy of <see cref="TgaHeader"/>.</returns>
        public TgaHeader Clone() => new TgaHeader(this.ToBytes());

        /// <summary>
        /// Make full copy of <see cref="TgaHeader"/>.
        /// </summary>
        /// <returns>Full independent copy of <see cref="TgaHeader"/>.</returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaHeader) ? this.Equals((TgaHeader)obj) : false);

        public bool Equals(TgaHeader item) => (this.idLength == item.idLength &&
                this.colorMapType == item.colorMapType &&
                this.imageType == item.imageType &&
                this.colorMapSpec == item.colorMapSpec &&
                this.imageSpec == item.imageSpec);

        public static bool operator ==(TgaHeader item1, TgaHeader item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaHeader item1, TgaHeader item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                var hash = 17;
                hash = hash * 23 + (this.idLength << 24 | (byte)this.colorMapType << 8 | (byte)this.imageType).GetHashCode();

                if(this.colorMapSpec != null)
                    hash = hash * 23 + this.colorMapSpec.GetHashCode();

                if(this.imageSpec != null)
                    hash = hash * 23 + this.imageSpec.GetHashCode();

                return hash;
            }
        }

        public override string ToString() => string.Format("{0}={1}, {2}={3}, {4}={5}, {6}={7}, {8}={9}",
                nameof(this.IDLength), this.idLength,
                nameof(this.ColorMapType), this.colorMapType,
                nameof(this.ImageType), this.imageType,
                nameof(this.ColorMapSpec), this.colorMapSpec,
                nameof(this.ImageSpec), this.imageSpec);

        /// <summary>
        /// Convert <see cref="TgaHeader"/> to byte array.
        /// </summary>
        /// <returns>Byte array with size equal <see cref="Size"/>.</returns>
        public byte[] ToBytes() => BitConverterExt.ToBytes(this.idLength, (byte)this.colorMapType, (byte)this.imageType,
                (this.colorMapSpec == null ? new byte[TgaColorMapSpec.Size] : this.colorMapSpec.ToBytes()),
                (this.imageSpec == null ? new byte[TgaImageSpec.Size] : this.imageSpec.ToBytes()));
    }

    /// <summary>
    /// Image Or ColorMap Area
    /// </summary>
    public class TgaImgOrColMap : ICloneable {
        private TgaString imageID = null;
        private byte[] colorMapData = null;
        private byte[] imageData = null;

        /// <summary>
        /// Make empty <see cref="TgaImgOrColMap"/>.
        /// </summary>
        public TgaImgOrColMap() {
        }

        /// <summary>
        /// Make <see cref="TgaImgOrColMap"/> from arrays.
        /// </summary>
        /// <param name="ImageID">This optional field contains identifying information about the image.
        /// The maximum length for this field is 255 bytes. Refer to <see cref="TgaHeader.IDLength"/>
        /// for the length of this field. If field 1 is set to Zero indicating that no Image ID exists
        /// then these bytes are not written to the file.</param>
        /// <param name="ColorMapData">Color Map Data, see <see cref="ColorMapData"/> description.</param>
        /// <param name="ImageData">Image Data, see <see cref="ImageData"/> description.</param>
        public TgaImgOrColMap(TgaString ImageID, byte[] ColorMapData, byte[] ImageData) {
            this.imageID = ImageID;
            this.colorMapData = ColorMapData;
            this.imageData = ImageData;
        }

        /// <summary>
        /// Image ID - Field 6 (variable):
        /// <para>This optional field contains identifying information about the image. The maximum length
        /// for this field is 255 bytes. Refer to <see cref="TgaHeader.IDLength"/> for the length of this
        /// field. If field 1 is set to Zero indicating that no Image ID exists then these bytes are not
        /// written to the file. Can have text inside (ASCII).</para>
        /// </summary>
        public TgaString ImageID {
            get => this.imageID;
            set => this.imageID = value;
        }

        /// <summary>
        /// Color Map Data - Field 7 (variable):
        /// <para>If the Color Map Type(field 2) field is set to zero indicating that no Color-Map
        /// exists then this field will not be present (i.e., no bytes written to the file).</para>
        /// <para>This variable-length field contains the actual color map information (LUT data).
        /// Field 4.3 specifies the width in bits of each color map entry while Field 4.2 specifies
        /// the number of color map entries in this field. These two fields together are used to
        /// determine the number of bytes contained in field 7.</para>
        /// <para>Each color map entry is stored using an integral number of bytes.The RGB specification
        /// for each color map entry is stored in successive bit-fields in the multi-byte entries.
        /// Each color bit-field is assumed to be MIN(Field4.3/3, 8) bits in length. If Field 4.3
        /// contains 24, then each color specification is 8 bits in length; if Field 4.3 contains 32,
        /// then each color specification is also 8 bits (32/3 gives 10, but 8 is smaller).
        /// Unused bit(s) in the multi-byte entries are assumed to specify attribute bits. The
        /// attribute bit field is often called the Alpha Channel, Overlay Bit(s) or Interrupt Bit(s).</para>
        /// For the TARGA M-8, ATVista and NuVista, the number of bits in a color map specification is
        /// 24 (or 32). The red, green, and blue components are each represented by one byte.
        /// </summary>
        public byte[] ColorMapData {
            get => this.colorMapData;
            set => this.colorMapData = value;
        }

        /// <summary>
        /// Image Data - Field 8 (variable):
        /// <para>This field contains (Width)x(Height) pixels. Each pixel specifies image data in one
        /// of the following formats:</para>
        /// <para>a single color-map index for Pseudo-Color;
        /// Attribute, Red, Green and Blue ordered data for True-Color;
        /// and independent color-map indices for Direct-Color.</para>
        /// <para>The values for Width and Height are specified in Fields 5.3 and 5.4 respectively.
        /// The number of attribute and color-definition bits for each pixel are defined in Fields 5.6
        /// and 5.5, respectively.Each pixel is stored as an integral number of bytes.</para>
        /// </summary>
        public byte[] ImageData {
            get => this.imageData;
            set => this.imageData = value;
        }

        /// <summary>
        /// Make full copy of <see cref="TgaImgOrColMap"/>.
        /// </summary>
        /// <returns>Full independed copy of <see cref="TgaImgOrColMap"/>.</returns>
        public TgaImgOrColMap Clone() => new TgaImgOrColMap(
                (this.imageID == null ? null : this.imageID.Clone()),
                (this.colorMapData == null ? null : (byte[])this.colorMapData.Clone()),
                (this.imageData == null ? null : (byte[])this.imageData.Clone()));

        /// <summary>
        /// Make full copy of <see cref="TgaImgOrColMap"/>.
        /// </summary>
        /// <returns>Full independed copy of <see cref="TgaImgOrColMap"/>.</returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaImgOrColMap) ? this.Equals((TgaImgOrColMap)obj) : false);

        public bool Equals(TgaImgOrColMap item) => this.imageID == item.imageID &&
                BitConverterExt.IsArraysEqual(this.colorMapData, item.colorMapData) &&
                BitConverterExt.IsArraysEqual(this.imageData, item.imageData);

        public static bool operator ==(TgaImgOrColMap item1, TgaImgOrColMap item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaImgOrColMap item1, TgaImgOrColMap item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                var hash = 27;

                if(this.imageID != null)
                    hash = (13 * hash) + this.imageID.GetHashCode();
                if(this.colorMapData != null) {
                    for(var i = 0; i < this.colorMapData.Length; i++)
                        hash = (13 * hash) + this.colorMapData[i].GetHashCode();
                }

                if(this.imageData != null) {
                    for(var i = 0; i < this.imageData.Length; i++)
                        hash = (13 * hash) + this.imageData[i].GetHashCode();
                }

                return hash;
            }
        }
    } //No ToBytes()

    /// <summary>
    /// Developer Area
    /// </summary> //?
    public class TgaDevArea : ICloneable {
        private List<TgaDevEntry> entries = new List<TgaDevEntry>();

        public TgaDevArea() {
        }

        public TgaDevArea(List<TgaDevEntry> Entries) {
            if(Entries == null)
                throw new ArgumentNullException(nameof(Entries) + " = null!");

            this.entries = Entries;
        }

        /// <summary>
        /// Developer Data - Field 9 (variable):
        /// </summary>
        public List<TgaDevEntry> Entries {
            get => this.entries;
            set => this.entries = value;
        }

        public int Count => this.entries.Count;

        public TgaDevEntry this[int index] {
            get => this.entries[index];
            set => this.entries[index] = value;
        }

        /// <summary>
        /// Make full copy of <see cref="TgaDevArea"/>.
        /// </summary>
        /// <returns>Full independent copy of <see cref="TgaDevArea"/>.</returns>
        public TgaDevArea Clone() {
            if(this.entries == null)
                return new TgaDevArea(null);

            var L = new List<TgaDevEntry>();
            for(var i = 0; i < this.entries.Count; i++)
                L.Add(this.entries[i].Clone());

            return new TgaDevArea(L);
        }

        /// <summary>
        /// Make full copy of <see cref="TgaDevArea"/>.
        /// </summary>
        /// <returns>Full independent copy of <see cref="TgaDevArea"/>.</returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaDevArea) ? this.Equals((TgaDevArea)obj) : false);

        public bool Equals(TgaDevArea item) => BitConverterExt.IsListsEqual(this.entries, item.entries);

        public static bool operator ==(TgaDevArea item1, TgaDevArea item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaDevArea item1, TgaDevArea item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                var hash = 27;
                if(this.entries != null) {
                    for(var i = 0; i < this.entries.Count; i++)
                        hash = (13 * hash) + this.entries[i].GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Convert <see cref="TgaDevArea"/> (without Fields Data, only Directory!) to byte array.
        /// </summary>
        /// <returns>Byte array, Len = (NUMBER_OF_TAGS_IN_THE_DIRECTORY * 10) + 2 bytes in size.
        /// The "+ 2" includes the 2 bytes for the number of tags in the directory.</returns>
        public byte[] ToBytes() {
            if(this.entries == null)
                throw new Exception(nameof(this.Entries) + " = null!");

            var NumberOfEntries = (ushort)Math.Min(ushort.MaxValue, this.entries.Count);
            var DevDir = new List<byte>(BitConverter.GetBytes(NumberOfEntries));

            for(var i = 0; i < this.entries.Count; i++) {
                DevDir.AddRange(BitConverter.GetBytes(this.entries[i].Tag));
                DevDir.AddRange(BitConverter.GetBytes(this.entries[i].Offset));
                DevDir.AddRange(BitConverter.GetBytes(this.entries[i].FieldSize));
            }

            return DevDir.ToArray();
        }
    } //Not full ToBytes()

    /// <summary>
    /// Extension Area
    /// </summary>
    public class TgaExtArea : ICloneable {
        public const int MinSize = 495; //bytes

        private ushort extensionSize = MinSize;
        private TgaString authorName = new TgaString(41, true);
        private TgaComment authorComments = new TgaComment();
        private TgaDateTime dateTimeStamp = new TgaDateTime();
        private TgaString jobNameOrID = new TgaString(41, true);
        private TgaTime jobTime = new TgaTime();
        private TgaString softwareID = new TgaString(41, true);
        private TgaSoftVersion softVersion = new TgaSoftVersion();
        private TgaColorKey keyColor = new TgaColorKey();
        private TgaFraction pixelAspectRatio = TgaFraction.Empty;
        private TgaFraction gammaValue = TgaFraction.Empty;
        private uint colorCorrectionOffset = 0;
        private uint postageStampOffset = 0;
        private uint scanLineOffset = 0;
        private TgaAttrType attributesType = TgaAttrType.NoAlpha;
        private uint[] scanLineTable = null;
        private TgaPostageStampImage postageStampImage = null;
        private ushort[] colorCorrectionTable = null;
        private byte[] otherDataInExtensionArea = null;

        public TgaExtArea() {
        }

        /// <summary>
        /// Make <see cref="TgaExtArea"/> from bytes. Warning: <see cref="ScanLineTable"/>,
        /// <see cref="PostageStampImage"/>, <see cref="ColorCorrectionTable"/> not included,
        /// because thea are can be not in the Extension Area of TGA file!
        /// </summary>
        /// <param name="Bytes">Bytes of <see cref="TgaExtArea"/>.</param>
        /// <param name="SLT">Scan Line Table.</param>
        /// <param name="PostImg">Postage Stamp Image.</param>
        /// <param name="CCT">Color Correction Table.</param>
        public TgaExtArea(byte[] Bytes, uint[] SLT = null, TgaPostageStampImage PostImg = null, ushort[] CCT = null) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            if(Bytes.Length < MinSize)
                throw new ArgumentOutOfRangeException(nameof(Bytes.Length) + " must be >= " + MinSize + "!");

            this.extensionSize = BitConverter.ToUInt16(Bytes, 0);
            this.authorName = new TgaString(BitConverterExt.GetElements(Bytes, 2, 41), true);
            this.authorComments = new TgaComment(BitConverterExt.GetElements(Bytes, 43, TgaComment.Size));
            this.dateTimeStamp = new TgaDateTime(BitConverterExt.GetElements(Bytes, 367, TgaDateTime.Size));
            this.jobNameOrID = new TgaString(BitConverterExt.GetElements(Bytes, 379, 41), true);
            this.jobTime = new TgaTime(BitConverterExt.GetElements(Bytes, 420, TgaTime.Size));
            this.softwareID = new TgaString(BitConverterExt.GetElements(Bytes, 426, 41), true);
            this.softVersion = new TgaSoftVersion(BitConverterExt.GetElements(Bytes, 467, TgaSoftVersion.Size));
            this.keyColor = new TgaColorKey(BitConverterExt.GetElements(Bytes, 470, TgaColorKey.Size));
            this.pixelAspectRatio = new TgaFraction(BitConverterExt.GetElements(Bytes, 474, TgaFraction.Size));
            this.gammaValue = new TgaFraction(BitConverterExt.GetElements(Bytes, 478, TgaFraction.Size));
            this.colorCorrectionOffset = BitConverter.ToUInt32(Bytes, 482);
            this.postageStampOffset = BitConverter.ToUInt32(Bytes, 486);
            this.scanLineOffset = BitConverter.ToUInt32(Bytes, 490);
            this.attributesType = (TgaAttrType)Bytes[494];

            if(this.extensionSize > MinSize)
                this.otherDataInExtensionArea = BitConverterExt.GetElements(Bytes, 495, Bytes.Length - MinSize);

            this.scanLineTable = SLT;
            this.postageStampImage = PostImg;
            this.colorCorrectionTable = CCT;
        }

        #region Properties
        /// <summary>
        /// Extension Size - Field 10 (2 Bytes):
        /// This field is a SHORT field which specifies the number of BYTES in the fixedlength portion of
        /// the Extension Area. For Version 2.0 of the TGA File Format, this number should be set to 495.
        /// If the number found in this field is not 495, then the file will be assumed to be of a
        /// version other than 2.0. If it ever becomes necessary to alter this number, the change
        /// will be controlled by Truevision, and will be accompanied by a revision to the TGA File
        /// Format with an accompanying change in the version number.
        /// </summary>
        public ushort ExtensionSize {
            get => this.extensionSize;
            set => this.extensionSize = value;
        }

        /// <summary>
        /// Author Name - Field 11 (41 Bytes):
        /// Bytes 2-42 - This field is an ASCII field of 41 bytes where the last byte must be a null
        /// (binary zero). This gives a total of 40 ASCII characters for the name. If the field is used,
        /// it should contain the name of the person who created the image (author). If the field is not
        /// used, you may fill it with nulls or a series of blanks(spaces) terminated by a null.
        /// The 41st byte must always be a null.
        /// </summary>
        public TgaString AuthorName {
            get => this.authorName;
            set => this.authorName = value;
        }

        /// <summary>
        /// Author Comments - Field 12 (324 Bytes):
        /// Bytes 43-366 - This is an ASCII field consisting of 324 bytes which are organized as four lines
        /// of 80 characters, each followed by a null terminator.This field is provided, in addition to the
        /// original IMAGE ID field(in the original TGA format), because it was determined that a few
        /// developers had used the IMAGE ID field for their own purposes.This field gives the developer
        /// four lines of 80 characters each, to use as an Author Comment area. Each line is fixed to 81
        /// bytes which makes access to the four lines easy.Each line must be terminated by a null.
        /// If you do not use all 80 available characters in the line, place the null after the last
        /// character and blank or null fill the rest of the line. The 81st byte of each of the four
        /// lines must be null.
        /// </summary>
        public TgaComment AuthorComments {
            get => this.authorComments;
            set => this.authorComments = value;
        }

        /// <summary>
        /// Date/Time Stamp - Field 13 (12 Bytes):
        /// Bytes 367-378 - This field contains a series of 6 SHORT values which define the integer
        /// value for the date and time that the image was saved. This data is formatted as follows:
        /// <para>SHORT 0: Month(1 - 12)</para>
        /// <para>SHORT 1: Day(1 - 31)</para>
        /// <para>SHORT 2: Year(4 digit, ie. 1989)</para>
        /// <para>SHORT 3: Hour(0 - 23)</para>
        /// <para>SHORT 4: Minute(0 - 59)</para>
        /// <para>SHORT 5: Second(0 - 59)</para>
        /// Even though operating systems typically time- and date-stamp files, this feature is
        /// provided because the operating system may change the time and date stamp if the file is
        /// copied. By using this area, you are guaranteed an unmodified region for date and time
        /// recording. If the fields are not used, you should fill them with binary zeros (0).
        /// </summary>
        public TgaDateTime DateTimeStamp {
            get => this.dateTimeStamp;
            set => this.dateTimeStamp = value;
        }

        /// <summary>
        /// Job Name/ID - Field 14 (41 Bytes):
        /// Bytes 379-419 - This field is an ASCII field of 41 bytes where the last byte must be 
        /// a binary zero. This gives a total of 40 ASCII characters for the job name or the ID.
        /// If the field is used, it should contain a name or id tag which refers to the job with
        /// which the image was associated.This allows production companies (and others) to tie
        /// images with jobs by using this field as a job name (i.e., CITY BANK) or job id number
        /// (i.e., CITY023). If the field is not used, you may fill it with a null terminated series
        /// of blanks (spaces) or nulls. In any case, the 41st byte must be a null.
        /// </summary>
        public TgaString JobNameOrID {
            get => this.jobNameOrID;
            set => this.jobNameOrID = value;
        }

        /// <summary>
        /// Job Time - Field 15 (6 Bytes):
        /// Bytes 420-425 - This field contains a series of 3 SHORT values which define the integer
        /// value for the job elapsed time when the image was saved.This data is formatted as follows:
        /// <para>SHORT 0: Hours(0 - 65535)</para>
        /// <para>SHORT 1: Minutes(0 - 59)</para>
        /// <para>SHORT 2: Seconds(0 - 59)</para>
        /// The purpose of this field is to allow production houses (and others) to keep a running total
        /// of the amount of time invested in a particular image. This may be useful for billing, costing,
        /// and time estimating. If the fields are not used, you should fill them with binary zeros (0).
        /// </summary>
        public TgaTime JobTime {
            get => this.jobTime;
            set => this.jobTime = value;
        }

        /// <summary>
        /// Software ID - Field 16 (41 Bytes):
        /// Bytes 426-466 - This field is an ASCII field of 41 bytes where the last byte must be
        /// a binary zero (null). This gives a total of 40 ASCII characters for the Software ID.
        /// The purpose of this field is to allow software to determine and record with what program
        /// a particular image was created.If the field is not used, you may fill it with a
        /// null terminated series of blanks (spaces) or nulls. The 41st byte must always be a null.
        /// </summary>
        public TgaString SoftwareID {
            get => this.softwareID;
            set => this.softwareID = value;
        }

        /// <summary>
        /// Software Version - Field 17 (3 Bytes):
        /// Bytes 467-469 - This field consists of two sub-fields, a SHORT and an ASCII BYTE.
        /// The purpose of this field is to define the version of software defined by the
        /// “Software ID” field above. The SHORT contains the version number as a binary
        /// integer times 100.
        /// <para>Therefore, software version 4.17 would be the integer value 417.This allows for
        /// two decimal positions of sub-version.The ASCII BYTE supports developers who also
        /// tag a release letter to the end. For example, if the version number is 1.17b, then
        /// the SHORT would contain 117. and the ASCII BYTE would contain “b”.
        /// The organization is as follows:</para>
        /// <para>SHORT (Bytes 0 - 1): Version Number * 100</para>
        /// <para>BYTE(Byte 2): Version Letter</para>
        /// If you do not use this field, set the SHORT to binary zero, and the BYTE to a space(“ “)
        /// </summary>
        public TgaSoftVersion SoftVersion {
            get => this.softVersion;
            set => this.softVersion = value;
        }

        /// <summary>
        /// Key Color - Field 18 (4 Bytes):
        /// Bytes 470-473 - This field contains a long value which is the key color in effect at
        /// the time the image is saved. The format is in A:R:G:B where ‘A’ (most significant byte)
        /// is the alpha channel key color(if you don’t have an alpha channel in your application,
        /// keep this byte zero [0]).
        /// <para>The Key Color can be thought of as the ‘background color’ or ‘transparent color’.
        /// This is the color of the ‘non image’ area of the screen, and the same color that the
        /// screen would be cleared to if erased in the application. If you don’t use this field,
        /// set it to all zeros (0). Setting the field to all zeros is the same as selecting a key
        /// color of black.</para>
        /// A good example of a key color is the ‘transparent color’ used in TIPS™ for WINDOW loading/saving.
        /// </summary>
        public TgaColorKey KeyColor {
            get => this.keyColor;
            set => this.keyColor = value;
        }

        /// <summary>
        /// Pixel Aspect Ratio - Field 19 (4 Bytes):
        /// Bytes 474-477 - This field contains two SHORT sub-fields, which when taken together
        /// specify a pixel size ratio.The format is as follows:
        /// <para>SHORT 0: Pixel Ratio Numerator(pixel width)</para>
        /// <para>SHORT 1: Pixel Ratio Denominator(pixel height)</para>
        /// These sub-fields may be used to determine the aspect ratio of a pixel. This is useful when
        /// it is important to preserve the proper aspect ratio of the saved image. If the two values
        /// are set to the same non-zero value, then the image is composed of square pixels. A zero
        /// in the second sub-field (denominator) indicates that no pixel aspect ratio is specified.
        /// </summary>
        public TgaFraction PixelAspectRatio {
            get => this.pixelAspectRatio;
            set => this.pixelAspectRatio = value;
        }

        /// <summary>
        /// Gamma Value - Field 20 (4 Bytes):
        /// Bytes 478-481 - This field contains two SHORT sub-fields, which when taken together in a ratio,
        /// provide a fractional gamma value.The format is as follows:
        /// <para>SHORT 0: Gamma Numerator</para>
        /// <para>SHORT 1: Gamma Denominator</para>
        /// The resulting value should be in the range of 0.0 to 10.0, with only one decimal place of
        /// precision necessary. An uncorrected image (an image with no gamma) should have the value 1.0 as
        /// the result.This may be accomplished by placing thesame, non-zero values in both positions
        /// (i.e., 1/1). If you decide to totally ignore this field, please set the denominator (the second
        /// SHORT) to the value zero. This will indicate that the Gamma Value field is not being used.
        /// </summary>
        public TgaFraction GammaValue {
            get => this.gammaValue;
            set => this.gammaValue = value;
        }

        /// <summary>
        /// Color Correction Offset - Field 21 (4 Bytes):
        /// Bytes 482-485 - This field is a 4-byte field containing a single offset value. This is an offset
        /// from the beginning of the file to the start of the Color Correction table. This table may be
        /// written anywhere between the end of the Image Data field (field 8) and the start of the TGA
        /// File Footer. If the image has no Color Correction Table or if the Gamma Value setting is
        /// sufficient, set this value to zero and do not write a Correction Table anywhere.
        /// </summary>
        public uint ColorCorrectionTableOffset {
            get => this.colorCorrectionOffset;
            set => this.colorCorrectionOffset = value;
        }

        /// <summary>
        /// Postage Stamp Offset - Field 22 (4 Bytes):
        /// Bytes 486-489 - This field is a 4-byte field containing a single offset value. This is an offset
        /// from the beginning of the file to the start of the Postage Stamp Image. The Postage Stamp Image
        /// must be written after Field 25 (Scan Line Table) but before the start of the TGA File Footer.
        /// If no postage stamp is stored, set this field to the value zero (0).
        /// </summary>
        public uint PostageStampOffset {
            get => this.postageStampOffset;
            set => this.postageStampOffset = value;
        }

        /// <summary>
        /// Scan Line Offset - Field 23 (4 Bytes):
        /// Bytes 490-493 - This field is a 4-byte field containing a single offset value. This is an
        /// offset from the beginning of the file to the start of the Scan Line Table.
        /// </summary>
        public uint ScanLineOffset {
            get => this.scanLineOffset;
            set => this.scanLineOffset = value;
        }

        /// <summary>
        /// Attributes Type - Field 24 (1 Byte):
        /// Byte 494 - This single byte field contains a value which specifies the type of Alpha channel
        /// data contained in the file. Value Meaning:
        /// <para>0: no Alpha data included (bits 3-0 of field 5.6 should also be set to zero)</para>
        /// <para>1: undefined data in the Alpha field, can be ignored</para>
        /// <para>2: undefined data in the Alpha field, but should be retained</para>
        /// <para>3: useful Alpha channel data is present</para>
        /// <para>4: pre-multiplied Alpha(see description below)</para>
        /// <para>5 -127: RESERVED</para>
        /// <para>128-255: Un-assigned</para>
        /// <para>Pre-multiplied Alpha Example: Suppose the Alpha channel data is being used to specify the
        /// opacity of each pixel(for use when the image is overlayed on another image), where 0 indicates
        /// that the pixel is completely transparent and a value of 1 indicates that the pixel is
        /// completely opaque(assume all component values have been normalized).</para>
        /// <para>A quadruple(a, r, g, b) of( 0.5, 1, 0, 0) would indicate that the pixel is pure red with a
        /// transparency of one-half. For numerous reasons(including image compositing) is is better to
        /// pre-multiply the individual color components with the value in the Alpha channel.</para>
        /// A pre-multiplication of the above would produce a quadruple(0.5, 0.5, 0, 0).
        /// A value of 3 in the Attributes Type Field(field 23) would indicate that the color components
        /// of the pixel have already been scaled by the value in the Alpha channel.
        /// </summary>
        public TgaAttrType AttributesType {
            get => this.attributesType;
            set => this.attributesType = value;
        }

        /// <summary>
        /// Scan Line Table - Field 25 (Variable):
        /// This information is provided, at the developers’ request, for two purposes:
        /// <para>1) To make random access of compressed images easy.</para>
        /// <para>2) To allow “giant picture” access in smaller “chunks”.</para>
        /// This table should contain a series of 4-byte offsets.Each offset you write should point to the
        /// start of the next scan line, in the order that the image was saved (i.e., top down or bottom up).
        /// The offset should be from the start of the file.Therefore, you will have a four byte value for
        /// each scan line in your image. This means that if your image is 768 pixels tall, you will have 768,
        /// 4-byte offset pointers (for a total of 3072 bytes). This size is not extreme, and thus this table
        /// can be built and maintained in memory, and then written out at the proper time.
        /// </summary>
        public uint[] ScanLineTable {
            get => this.scanLineTable;
            set => this.scanLineTable = value;
        }

        /// <summary>
        /// Postage Stamp Image - Field 26 (Variable):
        /// The Postage Stamp area is a smaller representation of the original image. This is useful for
        /// “browsing” a collection of image files. If your application can deal with a postage stamp image,
        /// it is recommended that you create one using sub-sampling techniques to create the best
        /// representation possible. The postage stamp image must be stored in the same format as the normal
        /// image specified in the file, but without any compression. The first byte of the postage stamp
        /// image specifies the X size of the stamp in pixels, the second byte of the stamp image specifies the
        /// Y size of the stamp in pixels. Truevision does not recommend stamps larger than 64x64 pixels, and
        /// suggests that any stamps stored larger be clipped. Obviously, the storage of the postage stamp
        /// relies heavily on the storage of the image. The two images are stored using the same format under
        /// the assumption that if you can read the image, then you can read the postage stamp. If the original
        /// image is color mapped, DO NOT average the postage stamp, as you will create new colors not in your map.
        /// </summary>
        public TgaPostageStampImage PostageStampImage {
            get => this.postageStampImage;
            set => this.postageStampImage = value;
        }

        /// <summary>
        /// Color Correction Table - Field 27 (2K Bytes):
        /// The Color Correction Table is a block of 256 x 4 SHORT values, where each set of
        /// four contiguous values are the desired A:R:G:B correction for that entry. This
        /// allows the user to store a correction table for image remapping or LUT driving.
        /// Since each color in the block is a SHORT, the maximum value for a color gun (i.e.,
        /// A, R, G, B) is 65535, and the minimum value is zero.Therefore, BLACK maps to
        /// 0, 0, 0, 0 and WHITE maps to 65535, 65535, 65535, 65535.
        /// </summary>
        public ushort[] ColorCorrectionTable {
            get => this.colorCorrectionTable;
            set => this.colorCorrectionTable = value;
        }

        /// <summary>
        /// Other Data In Extension Area (if <see cref="ExtensionSize"/> > 495).
        /// </summary>
        public byte[] OtherDataInExtensionArea {
            get => this.otherDataInExtensionArea;
            set => this.otherDataInExtensionArea = value;
        }
        #endregion

        /// <summary>
        /// Make full copy of <see cref="TgaExtArea"/>.
        /// </summary>
        /// <returns>Full independent copy of <see cref="TgaExtArea"/>.</returns>
        public TgaExtArea Clone() {
            var NewExtArea = new TgaExtArea {
                extensionSize = this.extensionSize,
                authorName = this.authorName.Clone(),
                authorComments = this.authorComments.Clone(),
                dateTimeStamp = this.dateTimeStamp.Clone(),
                jobNameOrID = this.jobNameOrID.Clone(),
                jobTime = this.jobTime.Clone(),
                softwareID = this.softwareID.Clone(),
                softVersion = this.softVersion.Clone(),
                keyColor = this.keyColor.Clone(),
                pixelAspectRatio = this.pixelAspectRatio.Clone(),
                gammaValue = this.gammaValue.Clone(),
                colorCorrectionOffset = this.colorCorrectionOffset,
                postageStampOffset = this.postageStampOffset,
                scanLineOffset = this.scanLineOffset,
                attributesType = this.attributesType
            };

            if(this.scanLineTable != null)
                NewExtArea.scanLineTable = (uint[])this.scanLineTable.Clone();
            if(this.postageStampImage != null)
                NewExtArea.postageStampImage = new TgaPostageStampImage(this.postageStampImage.ToBytes());
            if(this.colorCorrectionTable != null)
                NewExtArea.colorCorrectionTable = (ushort[])this.colorCorrectionTable.Clone();

            if(this.otherDataInExtensionArea != null)
                NewExtArea.otherDataInExtensionArea = (byte[])this.otherDataInExtensionArea.Clone();

            return NewExtArea;
        }

        /// <summary>
        /// Make full copy of <see cref="TgaExtArea"/>.
        /// </summary>
        /// <returns>Full independent copy of <see cref="TgaExtArea"/>.</returns>
        object ICloneable.Clone() => this.Clone();

        public override bool Equals(object obj) => ((obj is TgaExtArea) ? this.Equals((TgaExtArea)obj) : false);

        public bool Equals(TgaExtArea item) => (this.extensionSize == item.extensionSize &&
                this.authorName == item.authorName &&
                this.authorComments == item.authorComments &&
                this.dateTimeStamp == item.dateTimeStamp &&
                this.jobNameOrID == item.jobNameOrID &&
                this.jobTime == item.jobTime &&
                this.softwareID == item.softwareID &&
                this.softVersion == item.softVersion &&
                this.keyColor == item.keyColor &&
                this.pixelAspectRatio == item.pixelAspectRatio &&
                this.gammaValue == item.gammaValue &&
                this.colorCorrectionOffset == item.colorCorrectionOffset &&
                this.postageStampOffset == item.postageStampOffset &&
                this.scanLineOffset == item.scanLineOffset &&
                this.attributesType == item.attributesType &&

                BitConverterExt.IsArraysEqual(this.scanLineTable, item.scanLineTable) &&
                this.postageStampImage == item.postageStampImage &&
                BitConverterExt.IsArraysEqual(this.colorCorrectionTable, item.colorCorrectionTable) &&

                BitConverterExt.IsArraysEqual(this.otherDataInExtensionArea, item.otherDataInExtensionArea));

        public static bool operator ==(TgaExtArea item1, TgaExtArea item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaExtArea item1, TgaExtArea item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                var hash = 27;
                hash = (13 * hash) + this.extensionSize.GetHashCode();
                hash = (13 * hash) + this.authorName.GetHashCode();
                hash = (13 * hash) + this.authorComments.GetHashCode();
                hash = (13 * hash) + this.dateTimeStamp.GetHashCode();
                hash = (13 * hash) + this.jobNameOrID.GetHashCode();
                hash = (13 * hash) + this.jobTime.GetHashCode();
                hash = (13 * hash) + this.softwareID.GetHashCode();
                hash = (13 * hash) + this.softVersion.GetHashCode();
                hash = (13 * hash) + this.keyColor.GetHashCode();
                hash = (13 * hash) + this.pixelAspectRatio.GetHashCode();
                hash = (13 * hash) + this.gammaValue.GetHashCode();
                hash = (13 * hash) + this.colorCorrectionOffset.GetHashCode();
                hash = (13 * hash) + this.postageStampOffset.GetHashCode();
                hash = (13 * hash) + this.scanLineOffset.GetHashCode();
                hash = (13 * hash) + this.attributesType.GetHashCode();

                if(this.scanLineTable != null) {
                    for(var i = 0; i < this.scanLineTable.Length; i++)
                        hash = (13 * hash) + this.scanLineTable[i].GetHashCode();
                }

                if(this.postageStampImage != null)
                    hash = (13 * hash) + this.postageStampImage.GetHashCode();

                if(this.colorCorrectionTable != null) {
                    for(var i = 0; i < this.colorCorrectionTable.Length; i++)
                        hash = (13 * hash) + this.colorCorrectionTable[i].GetHashCode();
                }

                if(this.otherDataInExtensionArea != null) {
                    for(var i = 0; i < this.otherDataInExtensionArea.Length; i++)
                        hash = (13 * hash) + this.otherDataInExtensionArea[i].GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Convert <see cref="TgaExtArea"/> to byte array. Warning: <see cref="ScanLineTable"/>,
        /// <see cref="PostageStampImage"/>, <see cref="ColorCorrectionTable"/> not included,
        /// because thea are can be not in the Extension Area of TGA file!
        /// </summary>
        /// <returns>Byte array.</returns>
        public byte[] ToBytes() {
            #region Exceptions check
            if(this.authorName == null)
                this.authorName = new TgaString(41, true);

            if(this.authorComments == null)
                this.authorComments = new TgaComment();

            if(this.dateTimeStamp == null)
                this.dateTimeStamp = new TgaDateTime(DateTime.UtcNow);

            if(this.jobNameOrID == null)
                this.jobNameOrID = new TgaString(41, true);

            if(this.jobTime == null)
                this.jobTime = new TgaTime();

            if(this.softwareID == null)
                this.softwareID = new TgaString(41, true);

            if(this.softVersion == null)
                this.softVersion = new TgaSoftVersion();

            if(this.keyColor == null)
                this.keyColor = new TgaColorKey();

            if(this.pixelAspectRatio == null)
                this.pixelAspectRatio = new TgaFraction();

            if(this.gammaValue == null)
                this.gammaValue = new TgaFraction();
            #endregion

            return BitConverterExt.ToBytes(
                this.extensionSize,
                this.authorName.ToBytes(),
                this.authorComments.ToBytes(),
                this.dateTimeStamp.ToBytes(),
                this.jobNameOrID.ToBytes(),
                this.jobTime.ToBytes(),
                this.softwareID.ToBytes(),
                this.softVersion.ToBytes(),
                this.keyColor.ToBytes(),
                this.pixelAspectRatio.ToBytes(),
                this.gammaValue.ToBytes(),
                this.colorCorrectionOffset,
                this.postageStampOffset,
                this.scanLineOffset,
                (byte)this.attributesType,
                this.otherDataInExtensionArea);
        }
    } //Not full ToBytes()

    /// <summary>
    /// File Footer Area
    /// </summary>
    public class TgaFooter : ICloneable {
        private uint extAreaOffset = 0;
        private uint devDirOffset = 0;
        private TgaString signature = TgaString.XFileSignatute;
        private TgaString reservedChar = TgaString.DotSymbol;
        private TgaString zeroStrTerminator = TgaString.ZeroTerminator;

        /// <summary>
        /// Make NewXFile format TGA Footer with <see cref="ExtensionAreaOffset"/> = 0 and
        /// <see cref="DeveloperDirectoryOffset"/> = 0.
        /// </summary>
        public TgaFooter() {
        }

        /// <summary>
        /// Make <see cref="TgaFooter"/> from values.
        /// </summary>
        /// <param name="ExtOff">Extension Area Offset, offset from the beginning of the file.</param>
        /// <param name="DevDirOff">Developer Directory Offset, offset from the beginning of the file.</param>
        /// <param name="Sign">New TGA format signature.</param>
        /// <param name="ReservChr">Reserved Character - ASCII character “.” (period).</param>
        /// <param name="Termin">Binary Zero Terminator, a binary zero which acts as a final terminator.</param>
        public TgaFooter(uint ExtOff, uint DevDirOff, TgaString Sign, TgaString ReservChr, TgaString Termin) {
            this.extAreaOffset = ExtOff;
            this.devDirOffset = DevDirOff;
            this.signature = Sign;
            this.reservedChar = ReservChr;
            this.zeroStrTerminator = Termin;
        }

        /// <summary>
        /// Make <see cref="TgaFooter"/> from bytes (if signature is right).
        /// </summary>
        /// <param name="Bytes">Bytes array (byte[26]).</param>
        public TgaFooter(byte[] Bytes) {
            if(Bytes == null)
                throw new ArgumentNullException(nameof(Bytes) + " = null!");
            if(Bytes.Length != Size)
                throw new ArgumentOutOfRangeException(nameof(Bytes.Length) + " must be equal " + Size + "!");

            this.extAreaOffset = BitConverter.ToUInt32(Bytes, 0);
            this.devDirOffset = BitConverter.ToUInt32(Bytes, 4);
            this.signature = new TgaString(BitConverterExt.GetElements(Bytes, 8, TgaString.XFileSignatuteConst.Length));
            this.reservedChar = new TgaString(new byte[] { Bytes[24] });
            this.zeroStrTerminator = new TgaString(new byte[] { Bytes[25] });
        }

        /// <summary>
        /// Byte 0-3 - Extension Area Offset - Field 28
        /// The first four bytes (bytes 0-3, the first LONG) of the TGA File Footer contain an
        /// offset from the beginning of the file to the start of the Extension Area. Simply
        /// SEEK to this location to position to the start of the Extension Area. If the
        /// Extension Area Offset is zero, no Extension Area exists in the file.
        /// </summary>
        public uint ExtensionAreaOffset {
            get => this.extAreaOffset;
            set => this.extAreaOffset = value;
        }

        /// <summary>
        /// Byte 4-7 - Developer Directory Offset - Field 29
        /// The next four bytes(bytes 4-7, the second LONG) contain an offset from the
        /// beginning of the file to the start of the Developer Directory. If the Developer
        /// Directory Offset is zero, then the Developer Area does not exist.
        /// </summary>
        public uint DeveloperDirectoryOffset {
            get => this.devDirOffset;
            set => this.devDirOffset = value;
        }

        /// <summary>
        /// Byte 8-23 - Signature - Field 30
        /// This string is exactly 16 bytes long and is formatted exactly as shown below
        /// capital letters), with a hyphen between “TRUEVISION” and “XFILE.” If the
        /// signature is detected, the file is assumed to be of the New TGA format and MAY,
        /// therefore, contain the Developer Area and/or the Extension Area fields.If the
        /// signature is not found, then the file is assumed to be in the Original TGA format.
        /// </summary>
        public TgaString Signature {
            get => this.signature;
            set => this.signature = value;
        }

        /// <summary>
        /// Byte 24 - Reserved Character - Field 31
        /// Byte 24 is an ASCII character “.” (period). This character MUST BE a period or
        /// the file is not considered a proper TGA file.
        /// </summary>
        public TgaString ReservedCharacter {
            get => this.reservedChar;
            set => this.reservedChar = value;
        }

        /// <summary>
        /// Byte 25 - Binary Zero String Terminator - Field 32
        /// Byte 25 is a binary zero which acts as a final terminator and allows the entire TGA
        /// File Footer to be read and utilized as a “C” string.
        /// </summary>
        public TgaString BinaryZeroStringTerminator {
            get => this.zeroStrTerminator;
            set => this.zeroStrTerminator = value;
        }

        /// <summary>
        /// Make full copy of <see cref="TgaFooter"/>.
        /// </summary>
        /// <returns></returns>
        public TgaFooter Clone() => new TgaFooter(this.extAreaOffset, this.devDirOffset, this.signature.Clone(),
                this.reservedChar.Clone(), this.zeroStrTerminator.Clone());

        /// <summary>
        /// Make full copy of <see cref="TgaFooter"/>.
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone() => this.Clone();

        /// <summary>
        /// Gets TGA Footer Section size in bytes.
        /// </summary>
        public const int Size = 26;

        public override bool Equals(object obj) => ((obj is TgaFooter) ? this.Equals((TgaFooter)obj) : false);

        public bool Equals(TgaFooter item) => (this.extAreaOffset == item.extAreaOffset &&
                this.devDirOffset == item.devDirOffset &&
                this.signature == item.signature &&
                this.reservedChar == item.reservedChar &&
                this.zeroStrTerminator == item.zeroStrTerminator);

        public static bool operator ==(TgaFooter item1, TgaFooter item2) {
            if(ReferenceEquals(item1, null))
                return ReferenceEquals(item2, null);

            if(ReferenceEquals(item2, null))
                return ReferenceEquals(item1, null);

            return item1.Equals(item2);
        }

        public static bool operator !=(TgaFooter item1, TgaFooter item2) => !(item1 == item2);

        public override int GetHashCode() {
            unchecked {
                var hash = 17;
                hash = hash * 23 + this.extAreaOffset.GetHashCode();
                hash = hash * 23 + this.devDirOffset.GetHashCode();

                if(this.signature != null)
                    hash = hash * 23 + this.signature.GetHashCode();

                if(this.reservedChar != null)
                    hash = hash * 23 + this.reservedChar.GetHashCode();

                if(this.zeroStrTerminator != null)
                    hash = hash * 23 + this.zeroStrTerminator.GetHashCode();

                return hash;
            }
        }

        public override string ToString() => string.Format("{0}={1}, {2}={3}, FullSignature={4}",
                nameof(this.ExtensionAreaOffset), this.extAreaOffset, nameof(this.DeveloperDirectoryOffset), this.devDirOffset,
                (this.signature + this.reservedChar + this.zeroStrTerminator).ToString());

        /// <summary>
        /// Convert <see cref="TgaFooter"/> to byte array.
        /// </summary>
        /// <returns>Byte array with size equal <see cref="Size"/>.</returns>
        public byte[] ToBytes() => BitConverterExt.ToBytes(this.extAreaOffset, this.devDirOffset,
                this.signature.ToBytes(), this.reservedChar.ToBytes(), this.zeroStrTerminator.ToBytes());

        /// <summary>
        /// Is footer is real footer of TGA File Format Version 2.0?
        /// Checking by <see cref="TgaString.XFileSignatute"/>.
        /// </summary>
        public bool IsFooterCorrect => this.signature == TgaString.XFileSignatute;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Simplify ByteConversion operations, like concatination of byte arrays, comparing and other.
    /// </summary>
    public static class BitConverterExt {
        /// <summary>
        /// Combine byte, byte[], (u)short, (u)int, (u)long values to byte[] array.
        /// </summary>
        /// <param name="obj">Array of byte, byte[], (u)short, (u)int, (u)long values.</param>
        /// <returns>Array of bytes, null when some object is null.</returns>
        public static byte[] ToBytes(params object[] obj) {
            if(obj == null)
                return null;

            var BytesList = new List<byte>();

            for(var i = 0; i < obj.Length; i++) {
                if(obj[i] == null)
                    continue;
                else if(obj[i] is byte)
                    BytesList.Add((byte)obj[i]);
                else if(obj[i] is byte[])
                    BytesList.AddRange((byte[])obj[i]);
                else if(obj[i] is short)
                    BytesList.AddRange(BitConverter.GetBytes((short)obj[i]));
                else if(obj[i] is ushort)
                    BytesList.AddRange(BitConverter.GetBytes((ushort)obj[i]));
                else if(obj[i] is int)
                    BytesList.AddRange(BitConverter.GetBytes((int)obj[i]));
                else if(obj[i] is uint)
                    BytesList.AddRange(BitConverter.GetBytes((uint)obj[i]));
                else if(obj[i] is long)
                    BytesList.AddRange(BitConverter.GetBytes((long)obj[i]));
                else if(obj[i] is ulong)
                    BytesList.AddRange(BitConverter.GetBytes((ulong)obj[i]));
            }
            return BytesList.ToArray();
        }

        /// <summary>
        /// Copies a range of elements from an Array starting at the specified source index.
        /// The length and the index are specified as 32-bit integers.
        /// </summary>
        /// <param name="SrcArray">The <see cref="Array"/> that contains the data to copy.</param>
        /// <param name="Offset">A 32-bit integer that represents the index in the
        /// <see cref="SrcArray"/> at which copying begins.</param>
        /// <param name="Count">A 32-bit integer that represents the number of elements to copy.</param>
        /// <returns></returns>
        public static T[] GetElements<T>(T[] SrcArray, int Offset, int Count) {
            if(SrcArray == null)
                throw new ArgumentNullException(nameof(SrcArray) + " is null!");

            if(Offset >= SrcArray.Length || Offset < 0)
                throw new ArgumentOutOfRangeException(nameof(Offset) + " has wrong value!");

            if(Count <= 0 || Offset + Count > SrcArray.Length)
                throw new ArgumentOutOfRangeException(nameof(Count) + " has wrong value!");

            var Buff = new T[Count];
            Array.Copy(SrcArray, Offset, Buff, 0, Buff.Length);
            return Buff;
        }

        /// <summary>
        /// Compare N-dimensional Arrays.
        /// </summary>
        /// <typeparam name="T">Arrays Type.</typeparam>
        /// <param name="item1">First Array.</param>
        /// <param name="item2">Second Array.</param>
        /// <returns>True, if Arrays are equal.</returns>
        public static bool IsArraysEqual<T>(T[] item1, T[] item2) {
            if(ReferenceEquals(item1, item2))
                return true;

            if(item1 == null || item2 == null)
                return false;

            if(item1.Length != item2.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            for(var i = 0; i < item1.Length; i++) {
                if(!comparer.Equals(item1[i], item2[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Compare Lists.
        /// </summary>
        /// <typeparam name="T">List Type.</typeparam>
        /// <param name="item1">First List.</param>
        /// <param name="item2">Second List.</param>
        /// <returns>True, if Lists are equal.</returns>
        public static bool IsListsEqual<T>(List<T> item1, List<T> item2) {
            if(ReferenceEquals(item1, item2))
                return true;

            if(item1 == null || item2 == null)
                return false;

            if(item1.Count != item2.Count)
                return false;

            for(var i = 0; i < item1.Count; i++) {
                if(!item1[i].Equals(item2[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Compare elements in one Array with different offsets.
        /// </summary>
        /// <typeparam name="T">Array type.</typeparam>
        /// <param name="Arr">Some Array.</param>
        /// <param name="Offset1">First offset.</param>
        /// <param name="Offset2">Second offset.</param>
        /// <param name="Count">Elements count which must be compared.</param>
        /// <returns></returns>
        public static bool IsElementsEqual<T>(T[] Arr, int Offset1, int Offset2, int Count) {
            if(Arr == null)
                throw new ArgumentNullException(nameof(Arr) + " is null!");

            if(Offset1 >= Arr.Length || Offset1 < 0)
                throw new ArgumentOutOfRangeException(nameof(Offset1) + " has wrong value!");

            if(Offset2 >= Arr.Length || Offset2 < 0)
                throw new ArgumentOutOfRangeException(nameof(Offset2) + " has wrong value!");

            if(Count <= 0 || Offset1 + Count > Arr.Length || Offset2 + Count > Arr.Length)
                throw new ArgumentOutOfRangeException(nameof(Count) + " has wrong value!");

            if(Offset1 == Offset2)
                return true;

            for(var i = 0; i < Count; i++) {
                if(!Arr[Offset1 + i].Equals(Arr[Offset2 + i]))
                    return false;
            }

            return true;
        }
    }
    #endregion

    public class TGA : ICloneable {
        public TgaHeader Header = new TgaHeader();
        public TgaImgOrColMap ImageOrColorMapArea = new TgaImgOrColMap();
        public TgaDevArea DevArea = null;
        public TgaExtArea ExtArea = null;
        public TgaFooter Footer = null;

        #region TGA Creation, Loading, Saving (all are public, have reference to private metods).
        /// <summary>
        /// Create new empty <see cref="TGA"/> istance.
        /// </summary>
        public TGA() {
        }

        /// <summary>
        /// Create <see cref="TGA"/> instance with some params. If it must have ColorMap,
        /// check all ColorMap fields and settings after.
        /// </summary>
        /// <param name="Width">Image Width.</param>
        /// <param name="Height">Image Height.</param>
        /// <param name="PixDepth">Image Pixel Depth (bits / pixel), set ColorMap bpp after, if needed!</param>
        /// <param name="ImgType">Image Type (is RLE compressed, ColorMapped or GrayScaled).</param>
        /// <param name="AttrBits">Set numder of Attrbute bits (Alpha channel bits), default: 0, 1, 8.</param>
        /// <param name="NewFormat">Use new 2.0 TGA XFile format?</param>
        public TGA(ushort Width, ushort Height, TgaPixelDepth PixDepth = TgaPixelDepth.Bpp24,
            TgaImageType ImgType = TgaImageType.Uncompressed_TrueColor, byte AttrBits = 0, bool NewFormat = true) {
            if(Width <= 0 || Height <= 0 || PixDepth == TgaPixelDepth.Other) {
                Width = Height = 0;
                PixDepth = TgaPixelDepth.Other;
                ImgType = TgaImageType.NoImageData;
                AttrBits = 0;
            } else {
                var BytesPerPixel = (int)Math.Ceiling((double)PixDepth / 8.0);
                this.ImageOrColorMapArea.ImageData = new byte[Width * Height * BytesPerPixel];

                if(ImgType == TgaImageType.Uncompressed_ColorMapped || ImgType == TgaImageType.RLE_ColorMapped) {
                    this.Header.ColorMapType = TgaColorMapType.ColorMap;
                    this.Header.ColorMapSpec.FirstEntryIndex = 0;
                    this.Header.ColorMapSpec.ColorMapEntrySize = (TgaColorMapEntrySize)Math.Ceiling((double)PixDepth / 8);
                }
            }

            this.Header.ImageType = ImgType;
            this.Header.ImageSpec.ImageWidth = Width;
            this.Header.ImageSpec.ImageHeight = Height;
            this.Header.ImageSpec.PixelDepth = PixDepth;
            this.Header.ImageSpec.ImageDescriptor.AlphaChannelBits = AttrBits;

            if(NewFormat) {
                this.Footer = new TgaFooter();
                this.ExtArea = new TgaExtArea {
                    DateTimeStamp = new TgaDateTime(DateTime.UtcNow),
                    AttributesType = (AttrBits > 0 ? TgaAttrType.UsefulAlpha : TgaAttrType.NoAlpha)
                };
            }
        }

        /// <summary>
        /// Make <see cref="TGA"/> from some <see cref="TGA"/> instance.
        /// Equal to <see cref="TGA.Clone()"/> function.
        /// </summary>
        /// <param name="tga">Original <see cref="TGA"/> instance.</param>
        public TGA(TGA tga) {
            this.Header = tga.Header.Clone();
            this.ImageOrColorMapArea = tga.ImageOrColorMapArea.Clone();
            this.DevArea = tga.DevArea.Clone();
            this.ExtArea = tga.ExtArea.Clone();
            this.Footer = tga.Footer.Clone();
        }

        /// <summary>
        /// Load <see cref="TGA"/> from file.
        /// </summary>
        /// <param name="filename">Full path to TGA file.</param>
        /// <returns>Loaded <see cref="TGA"/> file.</returns>
        public TGA(string filename) => this.LoadFunc(filename);

        /// <summary>
        /// Make <see cref="TGA"/> from bytes array.
        /// </summary>
        /// <param name="bytes">Bytes array (same like TGA File).</param>
        public TGA(byte[] bytes) => this.LoadFunc(bytes);

        /// <summary>
        /// Make <see cref="TGA"/> from <see cref="Stream"/>.
        /// For file opening better use <see cref="FromFile(string)"/>.
        /// </summary>
        /// <param name="stream">Some stream. You can use a lot of Stream types, but Stream must support:
        /// <see cref="Stream.CanSeek"/> and <see cref="Stream.CanRead"/>.</param>
        public TGA(Stream stream) => this.LoadFunc(stream);

        /// <summary>
        /// Make <see cref="TGA"/> from <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bmp">Input Bitmap, supported a lot of bitmaps types: 8/15/16/24/32 Bpp's.</param>
        /// <param name="UseRLE">Use RLE Compression?</param>
        /// <param name="NewFormat">Use new 2.0 TGA XFile format?</param>
        /// <param name="ColorMap2BytesEntry">Is Color Map Entry size equal 15 or 16 Bpp, else - 24 or 32.</param>
        public TGA(Bitmap bmp, bool UseRLE = false, bool NewFormat = true, bool ColorMap2BytesEntry = false) => this.LoadFunc(bmp, UseRLE, NewFormat, ColorMap2BytesEntry);

        /// <summary>
        /// Load <see cref="TGA"/> from file.
        /// </summary>
        /// <param name="filename">Full path to TGA file.</param>
        /// <returns>Loaded <see cref="TGA"/> file.</returns>
        public static TGA FromFile(string filename) => new TGA(filename);

        /// <summary>
        /// Make <see cref="TGA"/> from bytes array.
        /// </summary>
        /// <param name="bytes">Bytes array (same like TGA File).</param>
        public static TGA FromBytes(byte[] bytes) => new TGA(bytes);

        /// <summary>
        /// Make <see cref="TGA"/> from <see cref="Stream"/>.
        /// For file opening better use <see cref="FromFile(string)"/>.
        /// </summary>
        /// <param name="stream">Some stream. You can use a lot of Stream types, but Stream must support:
        /// <see cref="Stream.CanSeek"/> and <see cref="Stream.CanRead"/>.</param>
        public static TGA FromStream(Stream stream) => new TGA(stream);

        /// <summary>
        /// Make <see cref="TGA"/> from <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bmp">Input Bitmap, supported a lot of bitmaps types: 8/15/16/24/32 Bpp's.</param>
        /// <param name="UseRLE">Use RLE Compression?</param>
        /// <param name="NewFormat">Use new 2.0 TGA XFile format?</param>
        /// <param name="ColorMap2BytesEntry">Is Color Map Entry size equal 15 or 16 Bpp, else - 24 or 32.</param>
        public static TGA FromBitmap(Bitmap bmp, bool UseRLE = false,
            bool NewFormat = true, bool ColorMap2BytesEntry = false) => new TGA(bmp, UseRLE, NewFormat, ColorMap2BytesEntry);

        /// <summary>
        /// Save <see cref="TGA"/> to file.
        /// </summary>
        /// <param name="filename">Full path to file.</param>
        /// <returns>Return "true", if all done or "false", if failed.</returns>
        public bool Save(string filename) {
            try {
                var Result = false;
                using(var Fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    using(var Ms = new MemoryStream()) {
                        Result = this.SaveFunc(Ms);
                        Ms.WriteTo(Fs);
                        Fs.Flush();
                    }
                }
                return Result;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Save <see cref="TGA"/> to <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Some stream, it must support: <see cref="Stream.CanWrite"/>.</param>
        /// <returns>Return "true", if all done or "false", if failed.</returns>
        public bool Save(Stream stream) => this.SaveFunc(stream);
        #endregion

        /// <summary>
        /// Gets or Sets Image Width (see <see cref="Header.ImageSpec.ImageWidth"/>).
        /// </summary>
        public ushort Width {
            get => this.Header.ImageSpec.ImageWidth;
            set => this.Header.ImageSpec.ImageWidth = value;
        }

        /// <summary>
        /// Gets or Sets Image Height (see <see cref="Header.ImageSpec.ImageHeight"/>).
        /// </summary>
        public ushort Height {
            get => this.Header.ImageSpec.ImageHeight;
            set => this.Header.ImageSpec.ImageHeight = value;
        }

        /// <summary>
        /// Gets or Sets <see cref="TGA"/> image Size.
        /// </summary>
        public Size Size {
            get => new Size(this.Header.ImageSpec.ImageWidth, this.Header.ImageSpec.ImageHeight);
            set {
                this.Header.ImageSpec.ImageWidth = (ushort)value.Width;
                this.Header.ImageSpec.ImageHeight = (ushort)value.Height;
            }
        }

        /// <summary>
        /// Make full independed copy of <see cref="TGA"/>.
        /// </summary>
        /// <returns>Full independed copy of <see cref="TGA"/>.</returns>
        public TGA Clone() => new TGA(this);

        object ICloneable.Clone() => this.Clone();

        /// <summary>
        /// Flip <see cref="TGA"/> directions, for more info see <see cref="TgaImgOrigin"/>.
        /// </summary>
        /// <param name="Horizontal">Flip horizontal.</param>
        /// <param name="Vertical">Flip vertical.</param>
        public void Flip(bool Horizontal = false, bool Vertical = false) {
            var NewOrigin = (int)this.Header.ImageSpec.ImageDescriptor.ImageOrigin;
            NewOrigin = NewOrigin ^ ((Vertical ? 0x20 : 0) | (Horizontal ? 0x10 : 0));
            this.Header.ImageSpec.ImageDescriptor.ImageOrigin = (TgaImgOrigin)NewOrigin;
        }

        /// <summary>
        /// Get information from TGA image.
        /// </summary>
        /// <returns>MultiLine string with info fields (one per line).</returns>
        public string GetInfo() {
            var SB = new StringBuilder();

            SB.AppendLine("Header:");
            SB.AppendLine("\tID Length = " + this.Header.IDLength);
            SB.AppendLine("\tImage Type = " + this.Header.ImageType);
            SB.AppendLine("\tHeader -> ImageSpec:");
            SB.AppendLine("\t\tImage Width = " + this.Header.ImageSpec.ImageWidth);
            SB.AppendLine("\t\tImage Height = " + this.Header.ImageSpec.ImageHeight);
            SB.AppendLine("\t\tPixel Depth = " + this.Header.ImageSpec.PixelDepth);
            SB.AppendLine("\t\tImage Descriptor (AsByte) = " + this.Header.ImageSpec.ImageDescriptor.ToByte());
            SB.AppendLine("\t\tImage Descriptor -> AttributeBits = " + this.Header.ImageSpec.ImageDescriptor.AlphaChannelBits);
            SB.AppendLine("\t\tImage Descriptor -> ImageOrigin = " + this.Header.ImageSpec.ImageDescriptor.ImageOrigin);
            SB.AppendLine("\t\tX_Origin = " + this.Header.ImageSpec.X_Origin);
            SB.AppendLine("\t\tY_Origin = " + this.Header.ImageSpec.Y_Origin);
            SB.AppendLine("\tColorMap Type = " + this.Header.ColorMapType);
            SB.AppendLine("\tHeader -> ColorMapSpec:");
            SB.AppendLine("\t\tColorMap Entry Size = " + this.Header.ColorMapSpec.ColorMapEntrySize);
            SB.AppendLine("\t\tColorMap Length = " + this.Header.ColorMapSpec.ColorMapLength);
            SB.AppendLine("\t\tFirstEntry Index = " + this.Header.ColorMapSpec.FirstEntryIndex);

            SB.AppendLine("\nImage / Color Map Area:");
            if(this.Header.IDLength > 0 && this.ImageOrColorMapArea.ImageID != null)
                SB.AppendLine("\tImage ID = \"" + this.ImageOrColorMapArea.ImageID.GetString() + "\"");
            else
                SB.AppendLine("\tImage ID = null");

            if(this.ImageOrColorMapArea.ImageData != null)
                SB.AppendLine("\tImage Data Length = " + this.ImageOrColorMapArea.ImageData.Length);
            else
                SB.AppendLine("\tImage Data = null");

            if(this.ImageOrColorMapArea.ColorMapData != null)
                SB.AppendLine("\tColorMap Data Length = " + this.ImageOrColorMapArea.ColorMapData.Length);
            else
                SB.AppendLine("\tColorMap Data = null");

            SB.AppendLine("\nDevelopers Area:");
            if(this.DevArea != null)
                SB.AppendLine("\tCount = " + this.DevArea.Count);
            else
                SB.AppendLine("\tDevArea = null");

            SB.AppendLine("\nExtension Area:");
            if(this.ExtArea != null) {
                SB.AppendLine("\tExtension Size = " + this.ExtArea.ExtensionSize);
                SB.AppendLine("\tAuthor Name = \"" + this.ExtArea.AuthorName.GetString() + "\"");
                SB.AppendLine("\tAuthor Comments = \"" + this.ExtArea.AuthorComments.GetString() + "\"");
                SB.AppendLine("\tDate / Time Stamp = " + this.ExtArea.DateTimeStamp);
                SB.AppendLine("\tJob Name / ID = \"" + this.ExtArea.JobNameOrID.GetString() + "\"");
                SB.AppendLine("\tJob Time = " + this.ExtArea.JobTime);
                SB.AppendLine("\tSoftware ID = \"" + this.ExtArea.SoftwareID.GetString() + "\"");
                SB.AppendLine("\tSoftware Version = \"" + this.ExtArea.SoftVersion + "\"");
                SB.AppendLine("\tKey Color = " + this.ExtArea.KeyColor);
                SB.AppendLine("\tPixel Aspect Ratio = " + this.ExtArea.PixelAspectRatio);
                SB.AppendLine("\tGamma Value = " + this.ExtArea.GammaValue);
                SB.AppendLine("\tColor Correction Table Offset = " + this.ExtArea.ColorCorrectionTableOffset);
                SB.AppendLine("\tPostage Stamp Offset = " + this.ExtArea.PostageStampOffset);
                SB.AppendLine("\tScan Line Offset = " + this.ExtArea.ScanLineOffset);
                SB.AppendLine("\tAttributes Type = " + this.ExtArea.AttributesType);

                if(this.ExtArea.ScanLineTable != null)
                    SB.AppendLine("\tScan Line Table = " + this.ExtArea.ScanLineTable.Length);
                else
                    SB.AppendLine("\tScan Line Table = null");

                if(this.ExtArea.PostageStampImage != null)
                    SB.AppendLine("\tPostage Stamp Image: " + this.ExtArea.PostageStampImage.ToString());
                else
                    SB.AppendLine("\tPostage Stamp Image = null");

                SB.AppendLine("\tColor Correction Table = " + (this.ExtArea.ColorCorrectionTable != null));
            } else {
                SB.AppendLine("\tExtArea = null");
            }

            SB.AppendLine("\nFooter:");
            if(this.Footer != null) {
                SB.AppendLine("\tExtension Area Offset = " + this.Footer.ExtensionAreaOffset);
                SB.AppendLine("\tDeveloper Directory Offset = " + this.Footer.DeveloperDirectoryOffset);
                SB.AppendLine("\tSignature (Full) = \"" + this.Footer.Signature.ToString() +
                    this.Footer.ReservedCharacter.ToString() + this.Footer.BinaryZeroStringTerminator.ToString() + "\"");
            } else {
                SB.AppendLine("\tFooter = null");
            }

            return SB.ToString();
        }

        /// <summary>
        /// Check and update all fields with data length and offsets.
        /// </summary>
        /// <returns>Return "true", if all OK or "false", if checking failed.</returns>
        public bool CheckAndUpdateOffsets(out string ErrorStr) {
            ErrorStr = string.Empty;

            if(this.Header == null) {
                ErrorStr = "Header = null";
                return false;
            }

            if(this.ImageOrColorMapArea == null) {
                ErrorStr = "ImageOrColorMapArea = null";
                return false;
            }

            uint Offset = TgaHeader.Size; // Virtual Offset

            #region Header
            if(this.ImageOrColorMapArea.ImageID != null) {
                var StrMaxLen = 255;
                if(this.ImageOrColorMapArea.ImageID.UseEndingChar)
                    StrMaxLen--;

                this.Header.IDLength = (byte)Math.Min(this.ImageOrColorMapArea.ImageID.OriginalString.Length, StrMaxLen);
                this.ImageOrColorMapArea.ImageID.Length = this.Header.IDLength;
                Offset += this.Header.IDLength;
            } else {
                this.Header.IDLength = 0;
            }
            #endregion

            #region ColorMap
            if(this.Header.ColorMapType != TgaColorMapType.NoColorMap) {
                if(this.Header.ColorMapSpec == null) {
                    ErrorStr = "Header.ColorMapSpec = null";
                    return false;
                }

                if(this.Header.ColorMapSpec.ColorMapLength == 0) {
                    ErrorStr = "Header.ColorMapSpec.ColorMapLength = 0";
                    return false;
                }

                if(this.ImageOrColorMapArea.ColorMapData == null) {
                    ErrorStr = "ImageOrColorMapArea.ColorMapData = null";
                    return false;
                }

                var CmBytesPerPixel = (int)Math.Ceiling((double)this.Header.ColorMapSpec.ColorMapEntrySize / 8.0);
                var LenBytes = this.Header.ColorMapSpec.ColorMapLength * CmBytesPerPixel;

                if(LenBytes != this.ImageOrColorMapArea.ColorMapData.Length) {
                    ErrorStr = "ImageOrColorMapArea.ColorMapData.Length has wrong size!";
                    return false;
                }

                Offset += (uint)this.ImageOrColorMapArea.ColorMapData.Length;
            }
            #endregion

            #region Image Data
            var BytesPerPixel = 0;
            if(this.Header.ImageType != TgaImageType.NoImageData) {
                if(this.Header.ImageSpec == null) {
                    ErrorStr = "Header.ImageSpec = null";
                    return false;
                }

                if(this.Header.ImageSpec.ImageWidth == 0 || this.Header.ImageSpec.ImageHeight == 0) {
                    ErrorStr = "Header.ImageSpec.ImageWidth = 0 or Header.ImageSpec.ImageHeight = 0";
                    return false;
                }

                if(this.ImageOrColorMapArea.ImageData == null) {
                    ErrorStr = "ImageOrColorMapArea.ImageData = null";
                    return false;
                }

                BytesPerPixel = (int)Math.Ceiling((double)this.Header.ImageSpec.PixelDepth / 8.0);
                if(this.Width * this.Height * BytesPerPixel != this.ImageOrColorMapArea.ImageData.Length) {
                    ErrorStr = "ImageOrColorMapArea.ImageData.Length has wrong size!";
                    return false;
                }

                if(this.Header.ImageType >= TgaImageType.RLE_ColorMapped &&
                    this.Header.ImageType <= TgaImageType.RLE_BlackWhite) {
                    var RLE = this.RLE_Encode(this.ImageOrColorMapArea.ImageData, this.Width, this.Height);
                    if(RLE == null) {
                        ErrorStr = "RLE Compressing error! Check Image Data size.";
                        return false;
                    }

                    Offset += (uint)RLE.Length;
                    RLE = null;
                } else {
                    Offset += (uint)this.ImageOrColorMapArea.ImageData.Length;
                }
            }
            #endregion

            #region Footer, DevArea, ExtArea
            if(this.Footer != null) {
                #region DevArea
                if(this.DevArea != null) {
                    var DevAreaCount = this.DevArea.Count;
                    for(var i = 0; i < DevAreaCount; i++) {
                        if(this.DevArea[i] == null || this.DevArea[i].FieldSize <= 0) //Del Empty Entries
                        {
                            this.DevArea.Entries.RemoveAt(i);
                            DevAreaCount--;
                            i--;
                        }
                    }

                    if(this.DevArea.Count <= 0)
                        this.Footer.DeveloperDirectoryOffset = 0;

                    if(this.DevArea.Count > 2) {
                        this.DevArea.Entries.Sort((a, b) => { return a.Tag.CompareTo(b.Tag); });
                        for(var i = 0; i < this.DevArea.Count - 1; i++) {
                            if(this.DevArea[i].Tag == this.DevArea[i + 1].Tag) {
                                ErrorStr = "DevArea Enties has same Tags!";
                                return false;
                            }
                        }
                    }

                    for(var i = 0; i < this.DevArea.Count; i++) {
                        this.DevArea[i].Offset = Offset;
                        Offset += (uint)this.DevArea[i].FieldSize;
                    }

                    this.Footer.DeveloperDirectoryOffset = Offset;
                    Offset += (uint)(this.DevArea.Count * 10 + 2);
                } else {
                    this.Footer.DeveloperDirectoryOffset = 0;
                }
                #endregion

                #region ExtArea
                if(this.ExtArea != null) {
                    this.ExtArea.ExtensionSize = TgaExtArea.MinSize;
                    if(this.ExtArea.OtherDataInExtensionArea != null)
                        this.ExtArea.ExtensionSize += (ushort)this.ExtArea.OtherDataInExtensionArea.Length;

                    this.ExtArea.DateTimeStamp = new TgaDateTime(DateTime.UtcNow);

                    this.Footer.ExtensionAreaOffset = Offset;
                    Offset += this.ExtArea.ExtensionSize;

                    #region ScanLineTable
                    if(this.ExtArea.ScanLineTable == null) {
                        this.ExtArea.ScanLineOffset = 0;
                    } else {
                        if(this.ExtArea.ScanLineTable.Length != this.Height) {
                            ErrorStr = "ExtArea.ScanLineTable.Length != Height";
                            return false;
                        }

                        this.ExtArea.ScanLineOffset = Offset;
                        Offset += (uint)(this.ExtArea.ScanLineTable.Length * 4);
                    }
                    #endregion

                    #region PostageStampImage
                    if(this.ExtArea.PostageStampImage == null) {
                        this.ExtArea.PostageStampOffset = 0;
                    } else {
                        if(this.ExtArea.PostageStampImage.Width == 0 || this.ExtArea.PostageStampImage.Height == 0) {
                            ErrorStr = "ExtArea.PostageStampImage Width or Height is equal 0!";
                            return false;
                        }

                        if(this.ExtArea.PostageStampImage.Data == null) {
                            ErrorStr = "ExtArea.PostageStampImage.Data == null";
                            return false;
                        }

                        var PImgSB = this.ExtArea.PostageStampImage.Width * this.ExtArea.PostageStampImage.Height * BytesPerPixel;
                        if(this.Header.ImageType != TgaImageType.NoImageData &&
                            this.ExtArea.PostageStampImage.Data.Length != PImgSB) {
                            ErrorStr = "ExtArea.PostageStampImage.Data.Length is wrong!";
                            return false;
                        }


                        this.ExtArea.PostageStampOffset = Offset;
                        Offset += (uint)(this.ExtArea.PostageStampImage.Data.Length);
                    }
                    #endregion

                    #region ColorCorrectionTable
                    if(this.ExtArea.ColorCorrectionTable == null) {
                        this.ExtArea.ColorCorrectionTableOffset = 0;
                    } else {
                        if(this.ExtArea.ColorCorrectionTable.Length != 1024) {
                            ErrorStr = "ExtArea.ColorCorrectionTable.Length != 256 * 4";
                            return false;
                        }

                        this.ExtArea.ColorCorrectionTableOffset = Offset;
                        Offset += (uint)(this.ExtArea.ColorCorrectionTable.Length * 2);
                    }
                    #endregion
                } else {
                    this.Footer.ExtensionAreaOffset = 0;
                }
                #endregion

                #region Footer
                if(this.Footer.ToBytes().Length != TgaFooter.Size) {
                    ErrorStr = "Footer.Length is wrong!";
                    return false;
                }

                Offset += TgaFooter.Size;
                #endregion
            }
            #endregion

            return true;
        }

        #region Convert
        /// <summary>
        /// Convert <see cref="TGA"/> to <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="ForceUseAlpha">Force use alpha channel.</param>
        /// <returns>Bitmap or null, on error.</returns>
        public Bitmap ToBitmap(bool ForceUseAlpha = false) => this.ToBitmapFunc(ForceUseAlpha, false);

        /// <summary>
        /// Convert <see cref="TGA"/> to bytes array.
        /// </summary>
        /// <returns>Bytes array, (equal to saved file, but in memory) or null (on error).</returns>
        public byte[] ToBytes() {
            try {
                byte[] Bytes;
                using(var ms = new MemoryStream()) {
                    this.Save(ms);
                    Bytes = ms.ToArray();
                    ms.Flush();
                }
                return Bytes;
            } catch {
                return null;
            }
        }

        /// <summary>
        /// Convert TGA Image to new XFile format (v2.0).
        /// </summary>
        public void ToNewFormat() {
            if(this.Footer == null)
                this.Footer = new TgaFooter();

            if(this.ExtArea == null) {
                this.ExtArea = new TgaExtArea {
                    DateTimeStamp = new TgaDateTime(DateTime.UtcNow)
                };

                if(this.Header.ImageSpec.ImageDescriptor.AlphaChannelBits > 0)
                    this.ExtArea.AttributesType = TgaAttrType.UsefulAlpha;
                else
                    this.ExtArea.AttributesType = TgaAttrType.NoAlpha;
            }
        }
        #endregion

        #region Private functions
        private bool LoadFunc(string filename) {
            if(!File.Exists(filename))
                throw new FileNotFoundException("File: \"" + filename + "\" not found!");

            try {
                using(var FS = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    return this.LoadFunc(FS);
            } catch {
                return false;
            }
        }

        private bool LoadFunc(byte[] bytes) {
            if(bytes == null)
                throw new ArgumentNullException();

            try {
                using(var FS = new MemoryStream(bytes, false))
                    return this.LoadFunc(FS);
            } catch {
                return false;
            }
        }

        private bool LoadFunc(Stream stream) {
            if(stream == null)
                throw new ArgumentNullException();
            if(!(stream.CanRead && stream.CanSeek))
                throw new FileLoadException("Stream reading or seeking is not avaiable!");

            try {
                stream.Seek(0, SeekOrigin.Begin);
                var Br = new BinaryReader(stream);

                this.Header = new TgaHeader(Br.ReadBytes(TgaHeader.Size));

                if(this.Header.IDLength > 0)
                    this.ImageOrColorMapArea.ImageID = new TgaString(Br.ReadBytes(this.Header.IDLength));

                if(this.Header.ColorMapSpec.ColorMapLength > 0) {
                    var CmBytesPerPixel = (int)Math.Ceiling((double)this.Header.ColorMapSpec.ColorMapEntrySize / 8.0);
                    var LenBytes = this.Header.ColorMapSpec.ColorMapLength * CmBytesPerPixel;
                    this.ImageOrColorMapArea.ColorMapData = Br.ReadBytes(LenBytes);
                }

                #region Read Image Data
                var BytesPerPixel = (int)Math.Ceiling((double)this.Header.ImageSpec.PixelDepth / 8.0);
                if(this.Header.ImageType != TgaImageType.NoImageData) {
                    var ImageDataSize = this.Width * this.Height * BytesPerPixel;
                    switch(this.Header.ImageType) {
                    case TgaImageType.RLE_ColorMapped:
                    case TgaImageType.RLE_TrueColor:
                    case TgaImageType.RLE_BlackWhite:

                        var DataOffset = 0;
                        byte PacketInfo;
                        int PacketCount;
                        byte[] RLE_Bytes, RLE_Part;
                        this.ImageOrColorMapArea.ImageData = new byte[ImageDataSize];

                        do {
                            PacketInfo = Br.ReadByte(); //1 type bit and 7 count bits. Len = Count + 1.
                            PacketCount = (PacketInfo & 127) + 1;

                            if(PacketInfo >= 128) // bit7 = 1, RLE
                            {
                                RLE_Bytes = new byte[PacketCount * BytesPerPixel];
                                RLE_Part = Br.ReadBytes(BytesPerPixel);
                                for(var i = 0; i < RLE_Bytes.Length; i++)
                                    RLE_Bytes[i] = RLE_Part[i % BytesPerPixel];
                            } else // RAW format
{
                                RLE_Bytes = Br.ReadBytes(PacketCount * BytesPerPixel);
                            }

                            Buffer.BlockCopy(RLE_Bytes, 0, this.ImageOrColorMapArea.ImageData, DataOffset, RLE_Bytes.Length);
                            DataOffset += RLE_Bytes.Length;
                        }
                        while(DataOffset < ImageDataSize);
                        RLE_Bytes = null;
                        break;

                    case TgaImageType.Uncompressed_ColorMapped:
                    case TgaImageType.Uncompressed_TrueColor:
                    case TgaImageType.Uncompressed_BlackWhite:
                        this.ImageOrColorMapArea.ImageData = Br.ReadBytes(ImageDataSize);
                        break;
                    }
                }
                #endregion

                #region Try parse Footer
                stream.Seek(-TgaFooter.Size, SeekOrigin.End);
                var FooterOffset = (uint)stream.Position;
                var MbFooter = new TgaFooter(Br.ReadBytes(TgaFooter.Size));
                if(MbFooter.IsFooterCorrect) {
                    this.Footer = MbFooter;
                    var DevDirOffset = this.Footer.DeveloperDirectoryOffset;
                    var ExtAreaOffset = this.Footer.ExtensionAreaOffset;

                    #region If Dev Area exist, read it.
                    if(DevDirOffset != 0) {
                        stream.Seek(DevDirOffset, SeekOrigin.Begin);
                        this.DevArea = new TgaDevArea();
                        uint NumberOfTags = Br.ReadUInt16();

                        var Tags = new ushort[NumberOfTags];
                        var TagOffsets = new uint[NumberOfTags];
                        var TagSizes = new uint[NumberOfTags];

                        for(var i = 0; i < NumberOfTags; i++) {
                            Tags[i] = Br.ReadUInt16();
                            TagOffsets[i] = Br.ReadUInt32();
                            TagSizes[i] = Br.ReadUInt32();
                        }

                        for(var i = 0; i < NumberOfTags; i++) {
                            stream.Seek(TagOffsets[i], SeekOrigin.Begin);
                            var Ent = new TgaDevEntry(Tags[i], TagOffsets[i], Br.ReadBytes((int)TagSizes[i]));
                            this.DevArea.Entries.Add(Ent);
                        }

                        Tags = null;
                        TagOffsets = null;
                        TagSizes = null;
                    }
                    #endregion

                    #region If Ext Area exist, read it.
                    if(ExtAreaOffset != 0) {
                        stream.Seek(ExtAreaOffset, SeekOrigin.Begin);
                        var ExtAreaSize = Math.Max((ushort)TgaExtArea.MinSize, Br.ReadUInt16());
                        stream.Seek(ExtAreaOffset, SeekOrigin.Begin);
                        this.ExtArea = new TgaExtArea(Br.ReadBytes(ExtAreaSize));

                        if(this.ExtArea.ScanLineOffset > 0) {
                            stream.Seek(this.ExtArea.ScanLineOffset, SeekOrigin.Begin);
                            this.ExtArea.ScanLineTable = new uint[this.Height];
                            for(var i = 0; i < this.ExtArea.ScanLineTable.Length; i++)
                                this.ExtArea.ScanLineTable[i] = Br.ReadUInt32();
                        }

                        if(this.ExtArea.PostageStampOffset > 0) {
                            stream.Seek(this.ExtArea.PostageStampOffset, SeekOrigin.Begin);
                            var W = Br.ReadByte();
                            var H = Br.ReadByte();
                            var ImgDataSize = W * H * BytesPerPixel;
                            if(ImgDataSize > 0)
                                this.ExtArea.PostageStampImage = new TgaPostageStampImage(W, H, Br.ReadBytes(ImgDataSize));
                        }

                        if(this.ExtArea.ColorCorrectionTableOffset > 0) {
                            stream.Seek(this.ExtArea.ColorCorrectionTableOffset, SeekOrigin.Begin);
                            this.ExtArea.ColorCorrectionTable = new ushort[256 * 4];
                            for(var i = 0; i < this.ExtArea.ColorCorrectionTable.Length; i++)
                                this.ExtArea.ColorCorrectionTable[i] = Br.ReadUInt16();
                        }
                    }
                    #endregion
                }
                #endregion

                Br.Close();
                return true;
            } catch {
                return false;
            }
        }

        private bool LoadFunc(Bitmap bmp, bool UseRLE = false, bool NewFormat = true, bool ColorMap2BytesEntry = false) {
            if(bmp == null)
                throw new ArgumentNullException();

            try {
                this.Header.ImageSpec.ImageWidth = (ushort)bmp.Width;
                this.Header.ImageSpec.ImageHeight = (ushort)bmp.Height;
                this.Header.ImageSpec.ImageDescriptor.ImageOrigin = TgaImgOrigin.TopLeft;

                switch(bmp.PixelFormat) {
                case PixelFormat.Indexed:
                case PixelFormat.Gdi:
                case PixelFormat.Alpha:
                case PixelFormat.Undefined:
                case PixelFormat.PAlpha:
                case PixelFormat.Extended:
                case PixelFormat.Max:
                case PixelFormat.Canonical:
                case PixelFormat.Format16bppRgb565:
                default:
                    throw new FormatException(nameof(PixelFormat) + " is not supported!");

                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format48bppRgb:
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:

                    var bpp = Math.Max(8, Image.GetPixelFormatSize(bmp.PixelFormat));
                    var BytesPP = bpp / 8;

                    if(bmp.PixelFormat == PixelFormat.Format16bppRgb555)
                        bpp = 15;

                    var IsAlpha = Image.IsAlphaPixelFormat(bmp.PixelFormat);
                    var IsPreAlpha = IsAlpha && bmp.PixelFormat.ToString().EndsWith("PArgb");
                    var IsColorMapped = bmp.PixelFormat.ToString().EndsWith("Indexed");

                    this.Header.ImageSpec.PixelDepth = (TgaPixelDepth)(BytesPP * 8);

                    if(IsAlpha) {
                        this.Header.ImageSpec.ImageDescriptor.AlphaChannelBits = (byte)(BytesPP * 2);

                        if(bmp.PixelFormat == PixelFormat.Format16bppArgb1555)
                            this.Header.ImageSpec.ImageDescriptor.AlphaChannelBits = 1;
                    }

                    #region ColorMap
                    var IsGrayImage = (bmp.PixelFormat == PixelFormat.Format16bppGrayScale | IsColorMapped);

                    if(IsColorMapped && bmp.Palette != null) {
                        var Colors = bmp.Palette.Entries;

                        #region Analyze ColorMapType
                        var AlphaSum = 0;
                        var ColorMapUseAlpha = false;

                        for(var i = 0; i < Colors.Length; i++) {
                            IsGrayImage &= (Colors[i].R == Colors[i].G && Colors[i].G == Colors[i].B);
                            ColorMapUseAlpha |= (Colors[i].A < 248);
                            AlphaSum |= Colors[i].A;
                        }
                        ColorMapUseAlpha &= (AlphaSum > 0);

                        var CMapBpp = (ColorMap2BytesEntry ? 15 : 24) + (ColorMapUseAlpha ? (ColorMap2BytesEntry ? 1 : 8) : 0);
                        var CMBytesPP = (int)Math.Ceiling(CMapBpp / 8.0);
                        #endregion

                        this.Header.ColorMapSpec.ColorMapLength = Math.Min((ushort)Colors.Length, ushort.MaxValue);
                        this.Header.ColorMapSpec.ColorMapEntrySize = (TgaColorMapEntrySize)CMapBpp;
                        this.ImageOrColorMapArea.ColorMapData = new byte[this.Header.ColorMapSpec.ColorMapLength * CMBytesPP];

                        var CMapEntry = new byte[CMBytesPP];

                        const float To5Bit = 32f / 256f; // Scale value from 8 to 5 bits.
                        for(var i = 0; i < Colors.Length; i++) {
                            switch(this.Header.ColorMapSpec.ColorMapEntrySize) {
                            case TgaColorMapEntrySize.A1R5G5B5:
                            case TgaColorMapEntrySize.X1R5G5B5:
                                var R = (int)(Colors[i].R * To5Bit);
                                var G = (int)(Colors[i].G * To5Bit) << 5;
                                var B = (int)(Colors[i].B * To5Bit) << 10;
                                var A = 0;

                                if(this.Header.ColorMapSpec.ColorMapEntrySize == TgaColorMapEntrySize.A1R5G5B5)
                                    A = ((Colors[i].A & 0x80) << 15);

                                CMapEntry = BitConverter.GetBytes(A | R | G | B);
                                break;

                            case TgaColorMapEntrySize.R8G8B8:
                                CMapEntry[0] = Colors[i].B;
                                CMapEntry[1] = Colors[i].G;
                                CMapEntry[2] = Colors[i].R;
                                break;

                            case TgaColorMapEntrySize.A8R8G8B8:
                                CMapEntry[0] = Colors[i].B;
                                CMapEntry[1] = Colors[i].G;
                                CMapEntry[2] = Colors[i].R;
                                CMapEntry[3] = Colors[i].A;
                                break;

                            case TgaColorMapEntrySize.Other:
                            default:
                                break;
                            }

                            Buffer.BlockCopy(CMapEntry, 0, this.ImageOrColorMapArea.ColorMapData, i * CMBytesPP, CMBytesPP);
                        }
                    }
                    #endregion

                    #region ImageType
                    if(UseRLE) {
                        if(IsGrayImage)
                            this.Header.ImageType = TgaImageType.RLE_BlackWhite;
                        else if(IsColorMapped)
                            this.Header.ImageType = TgaImageType.RLE_ColorMapped;
                        else
                            this.Header.ImageType = TgaImageType.RLE_TrueColor;
                    } else {
                        if(IsGrayImage)
                            this.Header.ImageType = TgaImageType.Uncompressed_BlackWhite;
                        else if(IsColorMapped)
                            this.Header.ImageType = TgaImageType.Uncompressed_ColorMapped;
                        else
                            this.Header.ImageType = TgaImageType.Uncompressed_TrueColor;
                    }

                    this.Header.ColorMapType = (IsColorMapped ? TgaColorMapType.ColorMap : TgaColorMapType.NoColorMap);
                    #endregion

                    #region NewFormat
                    if(NewFormat) {
                        this.Footer = new TgaFooter();
                        this.ExtArea = new TgaExtArea {
                            DateTimeStamp = new TgaDateTime(DateTime.UtcNow)
                        };

                        if(IsAlpha) {
                            this.ExtArea.AttributesType = TgaAttrType.UsefulAlpha;

                            if(IsPreAlpha)
                                this.ExtArea.AttributesType = TgaAttrType.PreMultipliedAlpha;
                        } else {
                            this.ExtArea.AttributesType = TgaAttrType.NoAlpha;

                            if(this.Header.ImageSpec.ImageDescriptor.AlphaChannelBits > 0)
                                this.ExtArea.AttributesType = TgaAttrType.UndefinedAlphaButShouldBeRetained;
                        }
                    }
                    #endregion

                    #region Bitmap width is aligned by 32 bits = 4 bytes! Delete it.
                    var StrideBytes = bmp.Width * BytesPP;
                    var PaddingBytes = (int)Math.Ceiling(StrideBytes / 4.0) * 4 - StrideBytes;

                    var ImageData = new byte[(StrideBytes + PaddingBytes) * bmp.Height];

                    var Re = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    var BmpData = bmp.LockBits(Re, ImageLockMode.ReadOnly, bmp.PixelFormat);
                    Marshal.Copy(BmpData.Scan0, ImageData, 0, ImageData.Length);
                    bmp.UnlockBits(BmpData);
                    BmpData = null;

                    if(PaddingBytes > 0) //Need delete bytes align
                    {
                        this.ImageOrColorMapArea.ImageData = new byte[StrideBytes * bmp.Height];
                        for(var i = 0; i < bmp.Height; i++) {
                            Buffer.BlockCopy(ImageData, i * (StrideBytes + PaddingBytes),
                                    this.ImageOrColorMapArea.ImageData, i * StrideBytes, StrideBytes);
                        }
                    } else {
                        this.ImageOrColorMapArea.ImageData = ImageData;
                    }

                    ImageData = null;

                    // Not official supported, but works (tested on 16bpp GrayScale test images)!
                    if(bmp.PixelFormat == PixelFormat.Format16bppGrayScale) {
                        for(long i = 0; i < this.ImageOrColorMapArea.ImageData.Length; i++)
                            this.ImageOrColorMapArea.ImageData[i] ^= byte.MaxValue;
                    }
                    #endregion

                    break;
                }

                return true;
            } catch {
                return false;
            }
        }

        private bool SaveFunc(Stream stream) {
            try {
                if(stream == null)
                    throw new ArgumentNullException();
                if(!(stream.CanWrite && stream.CanSeek))
                    throw new FileLoadException("Stream writing or seeking is not avaiable!");

                if(!this.CheckAndUpdateOffsets(out var CheckResult))
                    return false;

                var Bw = new BinaryWriter(stream);
                Bw.Write(this.Header.ToBytes());

                if(this.ImageOrColorMapArea.ImageID != null)
                    Bw.Write(this.ImageOrColorMapArea.ImageID.ToBytes());

                if(this.Header.ColorMapType != TgaColorMapType.NoColorMap)
                    Bw.Write(this.ImageOrColorMapArea.ColorMapData);

                #region ImageData
                if(this.Header.ImageType != TgaImageType.NoImageData) {
                    if(this.Header.ImageType >= TgaImageType.RLE_ColorMapped &&
                        this.Header.ImageType <= TgaImageType.RLE_BlackWhite) {
                        Bw.Write(this.RLE_Encode(this.ImageOrColorMapArea.ImageData, this.Width, this.Height));
                    } else {
                        Bw.Write(this.ImageOrColorMapArea.ImageData);
                    }
                }
                #endregion

                #region Footer
                if(this.Footer != null) {
                    #region DevArea
                    if(this.DevArea != null) {
                        for(var i = 0; i < this.DevArea.Count; i++)
                            Bw.Write(this.DevArea[i].Data);

                        Bw.Write((ushort)this.DevArea.Count);

                        for(var i = 0; i < this.DevArea.Count; i++) {
                            Bw.Write(this.DevArea[i].Tag);
                            Bw.Write(this.DevArea[i].Offset);
                            Bw.Write(this.DevArea[i].FieldSize);
                        }
                    }
                    #endregion

                    #region ExtArea
                    if(this.ExtArea != null) {
                        Bw.Write(this.ExtArea.ToBytes());

                        if(this.ExtArea.ScanLineTable != null) {
                            for(var i = 0; i < this.ExtArea.ScanLineTable.Length; i++)
                                Bw.Write(this.ExtArea.ScanLineTable[i]);
                        }

                        if(this.ExtArea.PostageStampImage != null)
                            Bw.Write(this.ExtArea.PostageStampImage.ToBytes());

                        if(this.ExtArea.ColorCorrectionTable != null) {
                            for(var i = 0; i < this.ExtArea.ColorCorrectionTable.Length; i++)
                                Bw.Write(this.ExtArea.ColorCorrectionTable[i]);
                        }
                    }
                    #endregion

                    Bw.Write(this.Footer.ToBytes());
                }
                #endregion

                Bw.Flush();
                stream.Flush();
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Encode image with RLE compression (used RLE per line)!
        /// </summary>
        /// <param name="ImageData">Image data, bytes array with size = Width * Height * BytesPerPixel.</param>
        /// <param name="Width">Image Width, must be > 0.</param>
        /// <param name="Height">Image Height, must be > 0.</param>
        /// <returns>Bytes array with RLE compressed image data.</returns>
        private byte[] RLE_Encode(byte[] ImageData, int Width, int Height) {
            if(ImageData == null)
                throw new ArgumentNullException(nameof(ImageData) + "in null!");

            if(Width <= 0 || Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(Width) + " and " + nameof(Height) + " must be > 0!");

            var Bpp = ImageData.Length / Width / Height; // Bytes per pixel
            var ScanLineSize = Width * Bpp;

            if(ScanLineSize * Height != ImageData.Length)
                throw new ArgumentOutOfRangeException("ImageData has wrong Length!");

            try {
                var Count = 0;
                var Pos = 0;
                var IsRLE = false;
                var Encoded = new List<byte>();
                var RowData = new byte[ScanLineSize];

                for(var y = 0; y < Height; y++) {
                    Pos = 0;
                    Buffer.BlockCopy(ImageData, y * ScanLineSize, RowData, 0, ScanLineSize);

                    while(Pos < ScanLineSize) {
                        if(Pos >= ScanLineSize - Bpp) {
                            Encoded.Add(0);
                            Encoded.AddRange(BitConverterExt.GetElements(RowData, Pos, Bpp));
                            Pos += Bpp;
                            break;
                        }

                        Count = 0; //1
                        IsRLE = BitConverterExt.IsElementsEqual(RowData, Pos, Pos + Bpp, Bpp);

                        for(var i = Pos + Bpp; i < Math.Min(Pos + 128 * Bpp, ScanLineSize) - Bpp; i += Bpp) {
                            if(IsRLE ^ BitConverterExt.IsElementsEqual(RowData, (IsRLE ? Pos : i), i + Bpp, Bpp)) {
                                //Count--;
                                break;
                            } else {
                                Count++;
                            }
                        }

                        var CountBpp = (Count + 1) * Bpp;
                        Encoded.Add((byte)(IsRLE ? Count | 128 : Count));
                        Encoded.AddRange(BitConverterExt.GetElements(RowData, Pos, (IsRLE ? Bpp : CountBpp)));
                        Pos += CountBpp;
                    }
                }

                return Encoded.ToArray();
            } catch {
                return null;
            }
        }

        /// <summary>
        /// Convert <see cref="TGA"/> to <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="ForceUseAlpha">Force use alpha channel.</param>
        /// <param name="PostageStampImage">Get Postage Stamp Image (Thumb) or get main image?</param>
        /// <returns>Bitmap or null, on error.</returns>
        private Bitmap ToBitmapFunc(bool ForceUseAlpha = false, bool PostageStampImage = false) {
            try {
                #region UseAlpha?
                var UseAlpha = true;
                if(this.ExtArea != null) {
                    switch(this.ExtArea.AttributesType) {
                    case TgaAttrType.NoAlpha:
                    case TgaAttrType.UndefinedAlphaCanBeIgnored:
                    case TgaAttrType.UndefinedAlphaButShouldBeRetained:
                        UseAlpha = false;
                        break;
                    case TgaAttrType.UsefulAlpha:
                    case TgaAttrType.PreMultipliedAlpha:
                    default:
                        break;
                    }
                }
                UseAlpha = (this.Header.ImageSpec.ImageDescriptor.AlphaChannelBits > 0 && UseAlpha) | ForceUseAlpha;
                #endregion

                #region IsGrayImage
                var IsGrayImage = this.Header.ImageType == TgaImageType.RLE_BlackWhite ||
                    this.Header.ImageType == TgaImageType.Uncompressed_BlackWhite;
                #endregion

                #region Get PixelFormat
                var PixFormat = PixelFormat.Format24bppRgb;

                switch(this.Header.ImageSpec.PixelDepth) {
                case TgaPixelDepth.Bpp8:
                    PixFormat = PixelFormat.Format8bppIndexed;
                    break;

                case TgaPixelDepth.Bpp16:
                    if(IsGrayImage)
                        PixFormat = PixelFormat.Format16bppGrayScale;
                    else
                        PixFormat = (UseAlpha ? PixelFormat.Format16bppArgb1555 : PixelFormat.Format16bppRgb555);
                    break;

                case TgaPixelDepth.Bpp24:
                    PixFormat = PixelFormat.Format24bppRgb;
                    break;

                case TgaPixelDepth.Bpp32:
                    if(UseAlpha) {
                        var f = this.Footer;
                        if(this.ExtArea?.AttributesType == TgaAttrType.PreMultipliedAlpha)
                            PixFormat = PixelFormat.Format32bppPArgb;
                        else
                            PixFormat = PixelFormat.Format32bppArgb;
                    } else {
                        PixFormat = PixelFormat.Format32bppRgb;
                    }

                    break;

                default:
                    PixFormat = PixelFormat.Undefined;
                    break;
                }
                #endregion

                var BMP_Width = (PostageStampImage ? this.ExtArea.PostageStampImage.Width : this.Width);
                var BMP_Height = (PostageStampImage ? this.ExtArea.PostageStampImage.Height : this.Height);
                var BMP = new Bitmap(BMP_Width, BMP_Height, PixFormat);

                #region ColorMap and GrayPalette
                if(this.Header.ColorMapType == TgaColorMapType.ColorMap &&
                   (this.Header.ImageType == TgaImageType.RLE_ColorMapped ||
                    this.Header.ImageType == TgaImageType.Uncompressed_ColorMapped)) {

                    var ColorMap = BMP.Palette;
                    var CMapColors = ColorMap.Entries;

                    switch(this.Header.ColorMapSpec.ColorMapEntrySize) {
                    case TgaColorMapEntrySize.X1R5G5B5:
                    case TgaColorMapEntrySize.A1R5G5B5:
                        const float To8Bit = 255f / 31f; // Scale value from 5 to 8 bits.
                        for(var i = 0; i < Math.Min(CMapColors.Length, this.Header.ColorMapSpec.ColorMapLength); i++) {
                            var A1R5G5B5 = BitConverter.ToUInt16(this.ImageOrColorMapArea.ColorMapData, i * 2);
                            var A = (UseAlpha ? (A1R5G5B5 & 0x8000) >> 15 : 1) * 255; // (0 or 1) * 255
                            var R = (int)(((A1R5G5B5 & 0x7C00) >> 10) * To8Bit);
                            var G = (int)(((A1R5G5B5 & 0x3E0) >> 5) * To8Bit);
                            var B = (int)((A1R5G5B5 & 0x1F) * To8Bit);
                            CMapColors[i] = Color.FromArgb(A, R, G, B);
                        }
                        break;

                    case TgaColorMapEntrySize.R8G8B8:
                        for(var i = 0; i < Math.Min(CMapColors.Length, this.Header.ColorMapSpec.ColorMapLength); i++) {
                            var Index = i * 3; //RGB = 3 bytes
                            int R = this.ImageOrColorMapArea.ColorMapData[Index + 2];
                            int G = this.ImageOrColorMapArea.ColorMapData[Index + 1];
                            int B = this.ImageOrColorMapArea.ColorMapData[Index];
                            CMapColors[i] = Color.FromArgb(R, G, B);
                        }
                        break;

                    case TgaColorMapEntrySize.A8R8G8B8:
                        for(var i = 0; i < Math.Min(CMapColors.Length, this.Header.ColorMapSpec.ColorMapLength); i++) {
                            var ARGB = BitConverter.ToInt32(this.ImageOrColorMapArea.ColorMapData, i * 4);
                            CMapColors[i] = Color.FromArgb(UseAlpha ? ARGB | (0xFF << 24) : ARGB);
                        }
                        break;

                    default:
                        ColorMap = null;
                        break;
                    }

                    if(ColorMap != null)
                        BMP.Palette = ColorMap;
                }

                if(PixFormat == PixelFormat.Format8bppIndexed && IsGrayImage) {
                    var GrayPalette = BMP.Palette;
                    var GrayColors = GrayPalette.Entries;
                    for(var i = 0; i < GrayColors.Length; i++)
                        GrayColors[i] = Color.FromArgb(i, i, i);
                    BMP.Palette = GrayPalette;
                }
                #endregion

                #region Bitmap width must by aligned (align value = 32 bits = 4 bytes)!
                byte[] ImageData;
                var BytesPerPixel = (int)Math.Ceiling((double)this.Header.ImageSpec.PixelDepth / 8.0);
                var StrideBytes = BMP.Width * BytesPerPixel;
                var PaddingBytes = (int)Math.Ceiling(StrideBytes / 4.0) * 4 - StrideBytes;

                if(PaddingBytes > 0) //Need bytes align
                {
                    ImageData = new byte[(StrideBytes + PaddingBytes) * BMP.Height];
                    for(var i = 0; i < BMP.Height; i++) {
                        Buffer.BlockCopy(PostageStampImage ? this.ExtArea.PostageStampImage.Data :
                            this.ImageOrColorMapArea.ImageData, i * StrideBytes, ImageData,
                            i * (StrideBytes + PaddingBytes), StrideBytes);
                    }
                } else {
                    ImageData = BitConverterExt.ToBytes(PostageStampImage ? this.ExtArea.PostageStampImage.Data :
                        this.ImageOrColorMapArea.ImageData);
                }

                // Not official supported, but works (tested on 2 test images)!
                if(PixFormat == PixelFormat.Format16bppGrayScale) {
                    for(long i = 0; i < ImageData.Length; i++)
                        ImageData[i] ^= byte.MaxValue;
                }
                #endregion

                var Re = new Rectangle(0, 0, BMP.Width, BMP.Height);
                var BmpData = BMP.LockBits(Re, ImageLockMode.WriteOnly, BMP.PixelFormat);
                Marshal.Copy(ImageData, 0, BmpData.Scan0, ImageData.Length);
                BMP.UnlockBits(BmpData);
                ImageData = null;
                BmpData = null;

                if(this.ExtArea != null && this.ExtArea.KeyColor.ToInt() != 0)
                    BMP.MakeTransparent(this.ExtArea.KeyColor.ToColor());

                #region Flip Image
                switch(this.Header.ImageSpec.ImageDescriptor.ImageOrigin) {
                case TgaImgOrigin.BottomLeft:
                    BMP.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;
                case TgaImgOrigin.BottomRight:
                    BMP.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    break;
                case TgaImgOrigin.TopLeft:
                default:
                    break;
                case TgaImgOrigin.TopRight:
                    BMP.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                }
                #endregion

                return BMP;
            } catch {
                return null;
            }
        }
        #endregion

        #region Explicit
        public static explicit operator Bitmap(TGA tga) => tga.ToBitmap();

        public static explicit operator TGA(Bitmap bmp) => FromBitmap(bmp);
        #endregion

        #region PostageStamp Image
        /// <summary>
        /// Convert <see cref="TgaPostageStampImage"/> to <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="ForceUseAlpha">Force use alpha channel.</param>
        /// <returns>Bitmap or null.</returns>
        public Bitmap GetPostageStampImage(bool ForceUseAlpha = false) {
            if(this.ExtArea == null || this.ExtArea.PostageStampImage == null || this.ExtArea.PostageStampImage.Data == null ||
                this.ExtArea.PostageStampImage.Width <= 0 || this.ExtArea.PostageStampImage.Height <= 0) {
                return null;
            }

            return this.ToBitmapFunc(ForceUseAlpha, true);
        }

        /// <summary>
        /// Update Postage Stamp Image or set it.
        /// </summary>
        public void UpdatePostageStampImage() {
            if(this.Header.ImageType == TgaImageType.NoImageData) {
                if(this.ExtArea != null)
                    this.ExtArea.PostageStampImage = null;
                return;
            }

            this.ToNewFormat();
            if(this.ExtArea.PostageStampImage == null)
                this.ExtArea.PostageStampImage = new TgaPostageStampImage();

            int PS_Width = this.Header.ImageSpec.ImageWidth;
            int PS_Height = this.Header.ImageSpec.ImageHeight;

            if(this.Width > 64 || this.Height > 64) {
                var AspectRatio = this.Width / (float)this.Height;
                PS_Width = (byte)(64f * (AspectRatio < 1f ? AspectRatio : 1f));
                PS_Height = (byte)(64f / (AspectRatio > 1f ? AspectRatio : 1f));
            }
            PS_Width = Math.Max(PS_Width, 4);
            PS_Height = Math.Max(PS_Height, 4);

            this.ExtArea.PostageStampImage.Width = (byte)PS_Width;
            this.ExtArea.PostageStampImage.Height = (byte)PS_Height;

            var BytesPerPixel = (int)Math.Ceiling((double)this.Header.ImageSpec.PixelDepth / 8.0);
            this.ExtArea.PostageStampImage.Data = new byte[PS_Width * PS_Height * BytesPerPixel];

            var WidthCoef = this.Width / (float)PS_Width;
            var HeightCoef = this.Height / (float)PS_Height;

            for(var y = 0; y < PS_Height; y++) {
                var Y_Offset = (int)(y * HeightCoef) * this.Width * BytesPerPixel;
                var y_Offset = y * PS_Width * BytesPerPixel;

                for(var x = 0; x < PS_Width; x++) {
                    Buffer.BlockCopy(this.ImageOrColorMapArea.ImageData, Y_Offset + (int)(x * WidthCoef) * BytesPerPixel,
                        this.ExtArea.PostageStampImage.Data, y_Offset + x * BytesPerPixel, BytesPerPixel);
                }
            }
        }

        public void DeletePostageStampImage() {
            if(this.ExtArea != null)
                this.ExtArea.PostageStampImage = null;
        }
        #endregion
    }
}
