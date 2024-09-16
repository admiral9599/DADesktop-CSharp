using Microsoft.Win32;
using System;

namespace DriveAdviser.Core
{
    public static class SiTag
    {
        public static bool TagExists => !string.IsNullOrEmpty(Tag);

        public static string Tag => (string)Registry.GetValue(Constants.TagLocation, "tag", string.Empty);

        //public SiTag()
        //{
        //    //Always set the beginning properties during instatiation
        //    ReadTag();


        //}


        public static void WriteTag(string tag)
        {
            //Tag equals null when the subkey doesn't exist. We make sure to create it.
            if (Tag == null)
            {
                Console.WriteLine("Creating Tag");
                CreateTagLocation();

            }

            Registry.SetValue(Constants.TagLocation, "tag", tag);
            //This sets the properties to the new values.
           // ReadTag();

        }


        //private static string ReadTag()
        //{
        //    Tag = (string)Registry.GetValue(Constants.TagLocation, "tag", string.Empty);
        //    TagExists = !string.IsNullOrEmpty(Tag);
        //    return Tag;
        //}



        private static void CreateTagLocation()
        {
            Registry.LocalMachine.CreateSubKey(Constants.TagSubKey);
        }
    }
}
