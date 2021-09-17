using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace alch_ingr_getter
{
    public class Settings
    {
        public Settings()
        {
            filename = "alch.ingredients";
        }

        [SettingName("Output Filename")]
        public string filename;
    }
}
