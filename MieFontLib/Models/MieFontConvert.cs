namespace MieFontLib
{
    public class MieFontConvert
    {
        public static MieFont Convert(
            MieFont en,
            MieFont.NFormatType formatTypeEN,
            MieFont jp,
            MieFont.NFormatType formatTypeJP,
            bool forceAdjustAscender)
        {
            MieFont newFont = new MieFont();

            newFont.Header = en.Header;
            newFont.FontEntries = jp.FontEntries;
            newFont.Footer = en.Footer;

            newFont.Header.Convert(
                jp.Header,
                formatTypeEN,
                forceAdjustAscender);
            newFont.Footer.Convert(jp.Footer, formatTypeJP);

            return newFont;
        }
    }
}
