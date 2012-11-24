using System;
using Eto.Drawing;
using System.Text;
#if IOS

using MonoTouch.UIKit;
using NSFont = MonoTouch.UIKit.UIFont;

namespace Eto.Platform.iOS.Drawing

#elif OSX

using MonoMac.AppKit;

namespace Eto.Platform.Mac.Drawing
	
#endif
{
	public static class FontExtensions
	{
		public static float LineHeight(this NSFont font)
		{
			var leading = Math.Floor (Math.Max (0, font.Leading) + 0.5f);
			var lineHeight = (float)(Math.Floor(font.Ascender + 0.5f) - Math.Floor (font.Descender + 0.5f) + leading);

			if (leading > 0)
				return lineHeight;
			else
				return (float)(lineHeight + Math.Floor(0.2 * lineHeight + 0.5));
		}
#if OSX
		public static NSFont ToNSFont (this Font font)
		{
			return ((FontHandler)font.Handler).Control;
		}
#endif
	}

	public class FontHandler : WidgetHandler<NSFont, Font>, IFont, IDisposable
	{
		public const float FONT_SIZE_FACTOR = 1.0F;
		FontFamily family;
		FontTypeface face;
		FontStyle? style;
		
		public FontHandler()
		{
		}

		public FontHandler (NSFont font)
		{
			this.Control = font;
		}

		public FontHandler (NSFont font, FontStyle style)
		{
			this.Control = font;
			this.style = style;
		}

		public void Create (FontTypeface face, float size)
		{
			this.face = face;
			this.family = face.Family;
			this.Control = ((FontTypefaceHandler)face.Handler).CreateFont(size);
		}
		
		public void Create (SystemFont systemFont, float? fontSize)
		{
			var size = fontSize;
			if (fontSize != null) size = size.Value * FONT_SIZE_FACTOR;
			switch (systemFont) {
			case SystemFont.Default:
				Control = NSFont.SystemFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.Bold:
				Control = NSFont.BoldSystemFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.Label:
#if IOS
				Control = NSFont.SystemFontOfSize(size ?? NSFont.LabelFontSize);
#elif OSX
				Control = NSFont.LabelFontOfSize(size ?? NSFont.LabelFontSize);
#endif
				break;
#if DESKTOP
			case SystemFont.TitleBar:
				Control = NSFont.TitleBarFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.ToolTip:
				Control = NSFont.ToolTipsFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.MenuBar:
				Control = NSFont.MenuBarFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.Menu:
				Control = NSFont.MenuFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.Message:
				Control = NSFont.MessageFontOfSize(size ?? NSFont.SystemFontSize);
				break;
			case SystemFont.Palette:
				Control = NSFont.PaletteFontOfSize(size ?? NSFont.SmallSystemFontSize);
				break;
			case SystemFont.StatusBar:
				Control = NSFont.SystemFontOfSize(size ?? NSFont.SystemFontSize);
				break;
#endif
			default:
				throw new NotSupportedException();
			}
		}

		public float LineHeight
		{
			get {
				return Control.LineHeight ();
			}
		}

        public void Create(string fontFamily, float sizeInPoints, FontStyle style)
        {
            Create(Eto.Drawing.FontFamily.Sans, sizeInPoints, FontStyle.Normal); // TODO
        }

		public void Create (FontFamily family, float size, FontStyle style)
		{
			
#if OSX
			this.style = style;
			this.family = family;

			NSFontTraitMask traits = style.ToNS ();
			var font = NSFontManager.SharedFontManager.FontWithFamily(family.Name, traits, 5, size * FONT_SIZE_FACTOR);
			if (font == null || font.Handle == IntPtr.Zero)
				throw new Exception(string.Format("Could not allocate font with family {0}, traits {1}, size {2}", family.Name, traits, size));
#elif IOS
			string suffix = string.Empty;
			var familyHandler = family.Handler as FontFamilyHandler;
			var font = familyHandler.CreateFont (size, style);
			/*
			var familyString = new StringBuilder();
			switch (family)
			{
			case Eto.Drawing.FontFamily.Monospace: familyString.Append ("CourierNewPS"); suffix = "MT"; break; 
			default:
            case Eto.Drawing.FontFamily.Sans: familyString.Append("Helvetica"); italicString = "Oblique"; break;
            case Eto.Drawing.FontFamily.Serif: familyString.Append("TimesNewRomanPS"); suffix = "MT"; break; 
			}
			bold = (style & FontStyle.Bold) != 0;
			italic = (style & FontStyle.Italic) != 0;
			
			if (bold || italic) familyString.Append ("-");
			if (bold) familyString.Append ("Bold");
			if (italic) familyString.Append (italicString);
			
			familyString.Append (suffix);
			var font = UIFont.FromName (familyString.ToString (), size);
			*/
#endif
			Control = font;
		}

		public string FamilyName
		{
			get { return Control.FamilyName; }
		}

		public FontFamily Family
		{
			get {
				if (family == null)
					family = new FontFamily(Widget.Generator, new FontFamilyHandler(Control.FamilyName));
				return family;
			}
		}

		public FontTypeface Typeface
		{
			get {
				if (face == null)
					face = ((FontFamilyHandler)Family.Handler).GetFace (Control);
				return face;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				if (style == null)
#if OSX
					style = NSFontManager.SharedFontManager.TraitsOfFont (Control).ToEto ();
#elif IOS
					style = Typeface.FontStyle;
#endif
				return style.Value;
			}
        }

        public bool Underline
        {
            get { return false;/* TODO */ }
        }

        public bool Strikeout
        {
            get { return false;/* TODO */ }
        }

        public float SizeInPoints
        {
            get { return 0f;/* TODO */ }
        }

        public string FontFamily
        {
            get { return "";/* TODO */ }
        }

        public float AscentInPixels
        {
            get { return 0f;/* TODO */ }
        }

        public float DescentInPixels
        {
            get { return 0f;/* TODO */ }
        }

        public float HeightInPixels
        {
            get { return 0f;/* TODO */ }
        }

        public float SizeInPixels
        {
            get { return Control.PointSize / FONT_SIZE_FACTOR; }
        }

        #region IFont Members

        public float ExHeightInPixels
        {
            get { return 0f;/* TODO */ }
        }

        #endregion


        public void Create()
        {
            /* TODO */
        }
    }
}