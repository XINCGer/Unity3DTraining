//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// FreeType library is a C++ library used to print text from TrueType fonts.
/// Since the code is in a native C++ DLL, you will need Unity Pro in order to use it.
/// FreeType project is open source and can be obtained from http://www.freetype.org/
/// 
/// A big thank you goes to Amplitude Studios for coming up with the idea on how to do
/// this in the first place. As a side note, their game "Endless Space" is pretty awesome!
/// http://www.amplitude-studios.com/
/// 
/// If you are curious where all these values come from, check the FreeType docs:
/// http://www.freetype.org/freetype2/docs/reference/ft2-base_interface.html
/// </summary>

static public class FreeType
{
	public const int Err_Cannot_Open_Resource = 1;
	public const int Err_Unknown_File_Format = 2;
	public const int Err_Invalid_File_Format = 3;
	public const int Err_Invalid_Version = 4;
	public const int Err_Lower_Module_Version = 5;
	public const int Err_Invalid_Argument = 6;
	public const int Err_Unimplemented_Feature = 7;
	public const int Err_Invalid_Table = 8;
	public const int Err_Invalid_Offset = 9;
	public const int Err_Invalid_Glyph_Index = 16;
	public const int Err_Invalid_Character_Code = 17;
	public const int Err_Invalid_Glyph_Format = 18;
	public const int Err_Cannot_Render_Glyph = 19;
	public const int Err_Invalid_Outline = 20;
	public const int Err_Invalid_Composite = 21;
	public const int Err_Too_Many_Hints = 22;
	public const int Err_Invalid_Pixel_Size = 23;
	public const int Err_Invalid_Handle = 32;
	public const int Err_Invalid_Library_Handle = 33;
	public const int Err_Invalid_Driver_Handle = 34;
	public const int Err_Invalid_Face_Handle = 35;
	public const int Err_Invalid_Size_Handle = 36;
	public const int Err_Invalid_Slot_Handle = 37;
	public const int Err_Invalid_CharMap_Handle = 38;
	public const int Err_Invalid_Cache_Handle = 39;
	public const int Err_Invalid_Stream_Handle = 40;
	public const int Err_Too_Many_Drivers = 48;
	public const int Err_Too_Many_Extensions = 49;
	public const int Err_Out_Of_Memory = 64;
	public const int Err_Unlisted_Object = 65;
	public const int Err_Cannot_Open_Stream = 81;
	public const int Err_Invalid_Stream_Seek = 82;
	public const int Err_Invalid_Stream_Skip = 83;
	public const int Err_Invalid_Stream_Read = 84;
	public const int Err_Invalid_Stream_Operation = 85;
	public const int Err_Invalid_Frame_Operation = 86;
	public const int Err_Nested_Frame_Access = 87;
	public const int Err_Invalid_Frame_Read = 88;
	public const int Err_Raster_Uninitialized = 96;
	public const int Err_Raster_Corrupted = 97;
	public const int Err_Raster_Overflow = 98;
	public const int Err_Raster_Negative_Height = 99;
	public const int Err_Too_Many_Caches = 112;
	public const int Err_Invalid_Opcode = 128;
	public const int Err_Too_Few_Arguments = 129;
	public const int Err_Stack_Overflow = 130;
	public const int Err_Code_Overflow = 131;
	public const int Err_Bad_Argument = 132;
	public const int Err_Divide_By_Zero = 133;
	public const int Err_Invalid_Reference = 134;
	public const int Err_Debug_OpCode = 135;
	public const int Err_ENDF_In_Exec_Stream = 136;
	public const int Err_Nested_DEFS = 137;
	public const int Err_Invalid_CodeRange = 138;
	public const int Err_Execution_Too_Long = 139;
	public const int Err_Too_Many_Function_Defs = 140;
	public const int Err_Too_Many_Instruction_Defs = 141;
	public const int Err_Table_Missing = 142;
	public const int Err_Horiz_Header_Missing = 143;
	public const int Err_Locations_Missing = 144;
	public const int Err_Name_Table_Missing = 145;
	public const int Err_CMap_Table_Missing = 146;
	public const int Err_Hmtx_Table_Missing = 147;
	public const int Err_Post_Table_Missing = 148;
	public const int Err_Invalid_Horiz_Metrics = 149;
	public const int Err_Invalid_CharMap_Format = 150;
	public const int Err_Invalid_PPem = 151;
	public const int Err_Invalid_Vert_Metrics = 152;
	public const int Err_Could_Not_Find_Context = 153;
	public const int Err_Invalid_Post_Table_Format = 154;
	public const int Err_Invalid_Post_Table = 155;
	public const int Err_Syntax_Error = 160;
	public const int Err_Stack_Underflow = 161;
	public const int Err_Ignore = 162;
	public const int Err_Missing_Startfont_Field = 176;
	public const int Err_Missing_Font_Field = 177;
	public const int Err_Missing_Size_Field = 178;
	public const int Err_Missing_Chars_Field = 179;
	public const int Err_Missing_Startchar_Field = 180;
	public const int Err_Missing_Encoding_Field = 181;
	public const int Err_Missing_Bbx_Field = 182;

	public const int FT_LOAD_CROP_BITMAP = 64;
	public const int FT_LOAD_DEFAULT = 0;
	public const int FT_LOAD_FORCE_AUTOHINT = 32;
	public const int FT_LOAD_IGNORE_GLOBAL_ADVANCE_WIDTH = 512;
	public const int FT_LOAD_IGNORE_TRANSFORM = 2048;
	public const int FT_LOAD_LINEAR_DESIGN = 8192;
	public const int FT_LOAD_MONOCHROME = 4096;
	public const int FT_LOAD_NO_BITMAP = 8;
	public const int FT_LOAD_NO_HINTING = 2;
	public const int FT_LOAD_NO_RECURSE = 1024;
	public const int FT_LOAD_NO_SCALE = 1;
	public const int FT_LOAD_PEDANTIC = 128;
	public const int FT_LOAD_RENDER = 4;
	public const int FT_LOAD_SBITS_ONLY = 16384;
	public const int FT_LOAD_VERTICAL_LAYOUT = 16;

	public enum FT_Glyph_Format
	{
		FT_GLYPH_FORMAT_NONE,
		FT_GLYPH_FORMAT_COMPOSITE = 1668246896,
		FT_GLYPH_FORMAT_BITMAP = 1651078259,
		FT_GLYPH_FORMAT_OUTLINE = 1869968492,
		FT_GLYPH_FORMAT_PLOTTER = 1886154612
	}

	public enum FT_Render_Mode
	{
		FT_RENDER_MODE_NORMAL,
		FT_RENDER_MODE_LIGHT,
		FT_RENDER_MODE_MONO,
		FT_RENDER_MODE_LCD,
		FT_RENDER_MODE_LCD_V,
		FT_RENDER_MODE_MAX
	}

	public struct FT_BBox
	{
		public int xMin;
		public int yMin;
		public int xMax;
		public int yMax;
	}

	public struct FT_Bitmap
	{
		public int rows;
		public int width;
		public int pitch;
		public IntPtr buffer;
		public short num_grays;
		public sbyte pixel_mode;
		public sbyte palette_mode;
		public IntPtr palette;
	}

	public struct FT_FaceRec
	{
		public int num_faces;
		public int face_index;
		public int face_flags;
		public int style_flags;
		public int num_glyphs;
		public IntPtr family_name;
		public IntPtr style_name;
		public int num_fixed_sizes;
		public IntPtr available_sizes;
		public int num_charmaps;
		public IntPtr charmaps;
		public FT_Generic generic;
		public FT_BBox bbox;
		public ushort units_per_EM;
		public short ascender;
		public short descender;
		public short height;
		public short max_advance_width;
		public short max_advance_height;
		public short underline_position;
		public short underline_thickness;
		public IntPtr glyph;
		public IntPtr size;
		public IntPtr charmap;
		public IntPtr driver;
		public IntPtr memory;
		public IntPtr stream;
		public FT_ListRec sizes_list;
		public FT_Generic autohint;
		public IntPtr extensions;
		public IntPtr _internal;
	}

	public struct FT_Generic
	{
		public IntPtr data;
		public IntPtr finalizer;
	}

	public struct FT_Glyph_Metrics
	{
		public int width;
		public int height;
		public int horiBearingX;
		public int horiBearingY;
		public int horiAdvance;
		public int vertBearingX;
		public int vertBearingY;
		public int vertAdvance;
	}

	public struct FT_GlyphSlotRec
	{
		public IntPtr library;
		public IntPtr face;
		public IntPtr next;
		public uint reserved;
		public FT_Generic generic;
		public FT_Glyph_Metrics metrics;
		public int linearHoriAdvance;
		public int linearVertAdvance;
		public FT_Vector advance;
		public FT_Glyph_Format format;
		public FT_Bitmap bitmap;
		public int bitmap_left;
		public int bitmap_top;
		public FT_Outline outline;
		public uint num_subglyphs;
		public IntPtr subglyphs;
		public IntPtr control_data;
		public int control_len;
		public int lsb_delta;
		public int rsb_delta;
		public IntPtr other;
		public IntPtr _internal;
	}

	public struct FT_ListRec
	{
		public IntPtr head;
		public IntPtr tail;
	}

	public struct FT_Outline
	{
		public short n_contours;
		public short n_points;
		public IntPtr points;
		public IntPtr tags;
		public IntPtr contours;
		public int flags;
	}

	public struct FT_Size_Metrics
	{
		public ushort x_ppem;
		public ushort y_ppem;
		public int x_scale;
		public int y_scale;
		public int ascender;
		public int descender;
		public int height;
		public int max_advance;
	}

	public struct FT_SizeRec
	{
		public IntPtr face;
		public FT_Generic generic;
		public FT_Size_Metrics metrics;
		public IntPtr _internal;
	}

	public struct FT_Vector
	{
		public int x;
		public int y;
	}
	
	static bool mFound = false;

	/// <summary>
	/// Whether the freetype library will be usable.
	/// </summary>

	static public bool isPresent
	{
		get
		{
			// According to Unity's documentation, placing the DLL into the Editor folder should make it possible
			// to use it from within the editor. However from all my testing, that does not appear to be the case.
			// The DLL has to be explicitly loaded first, or Unity doesn't seem to pick it up at all.
			// On Mac OS it doesn't seem to be possible to load it at all, unless it's located in /usr/local/lib.
			if (!mFound)
			{
				if (Application.platform == RuntimePlatform.WindowsEditor)
				{
					string path = NGUISettings.pathToFreeType;
					mFound = File.Exists(path);
					if (mFound) LoadLibrary(path);
				}
				else
				{
					string filename = "FreeType.dylib";

					if (File.Exists("/usr/local/lib/" + filename))
					{
						mFound = true;
					}
					else
					{
						string path = NGUISettings.pathToFreeType;

						if (File.Exists(path))
						{
							try
							{
								if (!System.IO.Directory.Exists("/usr/local/lib"))
									System.IO.Directory.CreateDirectory("/usr/local/lib");
								UnityEditor.FileUtil.CopyFileOrDirectory(path, "/usr/local/lib/" + filename);
								mFound = true;
							}
							catch (Exception ex)
							{
								Debug.LogWarning("Unable to copy " + filename + " to /usr/local/lib:\n" + ex.Message);
							}
						}
					}
				}
			}
			return mFound;
		}
	}

	/// <summary>
	/// Load the specified library.
	/// </summary>

	[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
	static extern IntPtr LoadLibrary (string lpFileName);

	/// <summary>
	/// Initialize the FreeType library. Must be called first before doing anything else.
	/// </summary>

	[DllImport("FreeType", CallingConvention = CallingConvention.Cdecl)]
	static extern int FT_Init_FreeType (out IntPtr library);

	/// <summary>
	/// Return the glyph index of a given character code. This function uses a charmap object to do the mapping.
	/// </summary>

	[DllImport("FreeType", CallingConvention = CallingConvention.Cdecl)]
	static extern uint FT_Get_Char_Index (IntPtr face, uint charcode);

	/// <summary>
	/// This function calls FT_Open_Face to open a font by its pathname.
	/// </summary>

	[DllImport("FreeType", CallingConvention = CallingConvention.Cdecl)]
	static extern int FT_New_Face (IntPtr library, string filepathname, int face_index, out IntPtr face);

	/// <summary>
	/// Discard a given face object, as well as all of its child slots and sizes.
	/// </summary>

	[DllImport("FreeType", CallingConvention = CallingConvention.Cdecl)]
	static extern int FT_Done_Face (IntPtr face);

	/// <summary>
	/// A function used to load a single glyph into the glyph slot of a face object.
	/// </summary>

	[DllImport("FreeType", CallingConvention = CallingConvention.Cdecl)]
	static extern int FT_Load_Glyph (IntPtr face, uint glyph_index, int load_flags);

	/// <summary>
	/// Convert a given glyph image to a bitmap.
	/// </summary>

	[DllImport("FreeType", CallingConvention = CallingConvention.Cdecl)]
	static extern int FT_Render_Glyph (ref FT_GlyphSlotRec slot, FT_Render_Mode render_mode);

	/// <summary>
	/// Retrieve kerning information for the specified pair of characters.
	/// </summary>

	[DllImport("FreeType", CallingConvention = CallingConvention.Cdecl)]
	static extern int FT_Get_Kerning (IntPtr face, uint left, uint right, uint kern_mode, out FT_Vector kerning);

	/// <summary>
	/// This function calls FT_Request_Size to request the nominal size (in pixels).
	/// </summary>

	[DllImport("FreeType", CallingConvention = CallingConvention.Cdecl)]
	static extern int FT_Set_Pixel_Sizes (IntPtr face, uint pixel_width, uint pixel_height);

	/// <summary>
	/// Notify FreeType that you are done using the library. Should be called at the end.
	/// </summary>

	[DllImport("FreeType", CallingConvention = CallingConvention.Cdecl)]
	static extern int FT_Done_FreeType (IntPtr library);

	/// <summary>
	/// Retrieve the list of faces from the specified font. For example: regular, bold, italic.
	/// </summary>

	static public string[] GetFaces (Font font)
	{
		if (font == null || !isPresent) return null;

		string[] names = null;
		IntPtr lib = IntPtr.Zero;
		IntPtr face = IntPtr.Zero;

		if (FT_Init_FreeType(out lib) != 0)
		{
			Debug.LogError("Failed to initialize FreeType");
			return null;
		}

		string fileName = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) +
			UnityEditor.AssetDatabase.GetAssetPath(font);

		if (File.Exists(fileName))
		{
			if (FT_New_Face(lib, fileName, 0, out face) != 0)
			{
				Debug.LogError("Unable to use the chosen font (FT_New_Face).");
			}
			else
			{
				FT_FaceRec record = (FT_FaceRec)Marshal.PtrToStructure(face, typeof(FT_FaceRec));
				names = new string[record.num_faces];

				for (int i = 0; i < record.num_faces; i++)
				{
					IntPtr ptr = IntPtr.Zero;

					if (FT_New_Face(lib, fileName, i, out ptr) == 0)
					{
						string family = Marshal.PtrToStringAnsi(record.family_name);
						string style = Marshal.PtrToStringAnsi(record.style_name);
						names[i] = family + " - " + style;
						FT_Done_Face(ptr);
					}
				}
			}
		}
		if (face != IntPtr.Zero) FT_Done_Face(face);
#if !UNITY_3_5
		if (lib != IntPtr.Zero) FT_Done_FreeType(lib);
#endif
		return names;
	}

	/// <summary>
	/// Create a bitmap font from the specified dynamic font.
	/// </summary>

	static public bool CreateFont (Font ttf, int size, int faceIndex, bool kerning, string characters, int padding, out BMFont font, out Texture2D tex)
	{
		font = null;
		tex = null;

		if (ttf == null || !isPresent) return false;

		IntPtr lib = IntPtr.Zero;
		IntPtr face = IntPtr.Zero;

		if (FT_Init_FreeType(out lib) != 0)
		{
			Debug.LogError("Failed to initialize FreeType");
			return false;
		}

		string fileName = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) +
			UnityEditor.AssetDatabase.GetAssetPath(ttf);

		if (!File.Exists(fileName))
		{
			Debug.LogError("Unable to use the chosen font.");
		}
		else if (FT_New_Face(lib, fileName, faceIndex, out face) != 0)
		{
			Debug.LogError("Unable to use the chosen font (FT_New_Face).");
		}
		else
		{
			font = new BMFont();
			font.charSize = size;

			Color32 white = Color.white;
			List<int> entries = new List<int>();
			List<Texture2D> textures = new List<Texture2D>();

			FT_FaceRec faceRec = (FT_FaceRec)Marshal.PtrToStructure(face, typeof(FT_FaceRec));
			FT_Set_Pixel_Sizes(face, 0, (uint)size);

			// Calculate the baseline value that would let the printed font be centered vertically
			//int ascender = (faceRec.met.ascender >> 6);
			//int descender = (faceRec.descender >> 6);
			//int baseline = ((ascender - descender) >> 1);
			//if ((baseline & 1) == 1) --baseline;

			//Debug.Log(ascender + " " + descender + " " + baseline);

			// Space character is not renderable
			FT_Load_Glyph(face, FT_Get_Char_Index(face, 32), FT_LOAD_DEFAULT);
			FT_GlyphSlotRec space = (FT_GlyphSlotRec)Marshal.PtrToStructure(faceRec.glyph, typeof(FT_GlyphSlotRec));

			// Space is not visible and doesn't have a texture
			BMGlyph spaceGlyph = font.GetGlyph(32, true);
			spaceGlyph.offsetX = 0;
			spaceGlyph.offsetY = 0;
			spaceGlyph.advance = (space.metrics.horiAdvance >> 6);
			spaceGlyph.channel = 15;
			spaceGlyph.x = 0;
			spaceGlyph.y = 0;
			spaceGlyph.width = 0;
			spaceGlyph.height = 0;

			// Save kerning information
			if (kerning)
			{
				for (int b = 0; b < characters.Length; ++b)
				{
					uint ch2 = characters[b];
					if (ch2 == 32) continue;

					FT_Vector vec;
					if (FT_Get_Kerning(face, ch2, 32, 0, out vec) != 0) continue;

					int offset = (vec.x >> 6);
					if (offset != 0) spaceGlyph.SetKerning((int)ch2, offset);
				}
			}

			// Run through all requested characters
			foreach (char ch in characters)
			{
				uint charIndex = FT_Get_Char_Index(face, (uint)ch);
				FT_Load_Glyph(face, charIndex, FT_LOAD_DEFAULT);
				FT_GlyphSlotRec glyph = (FT_GlyphSlotRec)Marshal.PtrToStructure(faceRec.glyph, typeof(FT_GlyphSlotRec));
				FT_Render_Glyph(ref glyph, FT_Render_Mode.FT_RENDER_MODE_NORMAL);

				if (glyph.bitmap.width > 0 && glyph.bitmap.rows > 0)
				{
					byte[] buffer = new byte[glyph.bitmap.width * glyph.bitmap.rows];
					Marshal.Copy(glyph.bitmap.buffer, buffer, 0, buffer.Length);

					Texture2D texture = new Texture2D(glyph.bitmap.width, glyph.bitmap.rows, UnityEngine.TextureFormat.ARGB32, false);
					Color32[] colors = new Color32[buffer.Length];

					for (int i = 0, y = 0; y < glyph.bitmap.rows; ++y)
					{
						for (int x = 0; x < glyph.bitmap.width; ++x)
						{
							white.a = buffer[i++];
							colors[x + glyph.bitmap.width * (glyph.bitmap.rows - y - 1)] = white;
						}
					}

					// Save the texture
					texture.SetPixels32(colors);
					texture.Apply();
					textures.Add(texture);
					entries.Add(ch);

					// Record the metrics
					BMGlyph bmg = font.GetGlyph(ch, true);
					bmg.offsetX = (glyph.metrics.horiBearingX >> 6);
					bmg.offsetY = -(glyph.metrics.horiBearingY >> 6);
					bmg.advance = (glyph.metrics.horiAdvance >> 6);
					bmg.channel = 15;

					// Save kerning information
					if (kerning)
					{
						for (int b = 0; b < characters.Length; ++b)
						{
							uint ch2 = characters[b];
							if (ch2 == ch) continue;

							FT_Vector vec;
							if (FT_Get_Kerning(face, ch2, ch, 0, out vec) != 0) continue;

							int offset = (vec.x >> 6);
							if (offset != 0) bmg.SetKerning((int)ch2, offset);
						}
					}
				}
			}

			// Create a packed texture with all the characters
			tex = new Texture2D(32, 32, TextureFormat.ARGB32, false);
			Rect[] rects = tex.PackTextures(textures.ToArray(), padding);

			// Make the RGB channel pure white
			Color32[] cols = tex.GetPixels32();
			for (int i = 0, imax = cols.Length; i < imax; ++i)
			{
				Color32 c = cols[i];
				c.r = 255;
				c.g = 255;
				c.b = 255;
				cols[i] = c;
			}
			tex.SetPixels32(cols);
			tex.Apply();

			font.texWidth = tex.width;
			font.texHeight = tex.height;

			int min = int.MaxValue;
			int max = int.MinValue;

			// Other glyphs are visible and need to be added
			for (int i = 0, imax = entries.Count; i < imax; ++i)
			{
				// Destroy the texture now that it's a part of an atlas
				UnityEngine.Object.DestroyImmediate(textures[i]);
				textures[i] = null;
				Rect rect = rects[i];

				// Set the texture coordinates
				BMGlyph glyph = font.GetGlyph(entries[i], true);
				glyph.x = Mathf.RoundToInt(rect.x * font.texWidth);
				glyph.y = Mathf.RoundToInt(rect.y * font.texHeight);
				glyph.width = Mathf.RoundToInt(rect.width * font.texWidth);
				glyph.height = Mathf.RoundToInt(rect.height * font.texHeight);

				// Flip the Y since the UV coordinate system is different
				glyph.y = font.texHeight - glyph.y - glyph.height;

				max = Mathf.Max(max, -glyph.offsetY);
				min = Mathf.Min(min, -glyph.offsetY - glyph.height);
			}

			int baseline = size + min;
			baseline += ((max - min - size) >> 1);

			// Offset all glyphs so that they are not using the baseline
			for (int i = 0, imax = entries.Count; i < imax; ++i)
			{
				BMGlyph glyph = font.GetGlyph(entries[i], true);
				glyph.offsetY += baseline;
			}
		}
		
		if (face != IntPtr.Zero) FT_Done_Face(face);
#if !UNITY_3_5
		if (lib != IntPtr.Zero) FT_Done_FreeType(lib);
#endif
		return (tex != null);
	}
}
