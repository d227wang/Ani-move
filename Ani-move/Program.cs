using System;
using System.Collections.Generic;
using System.Linq;


class Animove
{
    static void animeProcess( string fileName, out string Title, out string Episode )
    {
        fileName.Replace( "_", " " );
        String FansubGroup = fileName.Substring( 1, fileName.IndexOf( "]" ) - 1 );

        String Temp = fileName.Substring( fileName.IndexOf( "]" ) + 1 );
        Temp = Temp.Trim( );

        Title = Temp.Substring( 0, Temp.LastIndexOf( " -" ) );
        if (!(Temp.Contains( "[" )))
        {
            Episode = Temp.Substring( Temp.LastIndexOf( "- " ) + 2 );
            Episode = Episode.Substring( 0, Episode.Length - 4 );
            Episode = Episode.TrimStart( '0' );
        }
        else
        {
            Episode = Temp.Substring( Temp.LastIndexOf( "- " ) + 2 );
            Episode = Episode.Substring( 0, Episode.IndexOf( "[" ) );
            Episode = Episode.Trim( );
            Episode = Episode.TrimStart( '0' );
        }
    }

    static void Main( string[] args )
    {
        Console.WriteLine( "[Ani-move]" );
        Console.WriteLine( "Source code available at github.com/moonrobin" );
        Console.WriteLine( "Build 1.0.1");
        Console.WriteLine( "=============================================");
        Console.WriteLine( );

        string animeDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyVideos ) + "\\Anime";
        Console.Write( "Use default anime directory at: " + animeDirectory + "? Y/N " );
        string input = Console.ReadLine( );
        var validInputs = new List<string>( ) { "Y", "N", "n", "y", "" };

        while (!(validInputs.Contains( input )))
        {
            Console.WriteLine( "Not a valid input." );
            input = Console.ReadLine( );
        }

        if (input == "n" || input == "N")
        {
            Console.WriteLine( "Please anime directory's root folder path: " );
            animeDirectory = Console.ReadLine( );

            // Check to see if user entered string is a valid path
            while (System.IO.Directory.Exists( animeDirectory ) == false)
            {
                Console.WriteLine( "Entered path is either invalid, or does not exist." );
                animeDirectory = Console.ReadLine( );
            }
            animeDirectory += "\\Anime";
        }

        bool animeDirectoryExists = System.IO.Directory.Exists( animeDirectory );

        // Creates directory if it doesn't currently exist
        if (animeDirectoryExists == false)
        {
            Console.Write( "Anime directory does not exist yet, should I create it? Y/N " );
            input = Console.ReadLine( );
            while (!(validInputs.Contains( input )))
            {
                Console.WriteLine( "Not a valid input." );
                input = Console.ReadLine( );
            }

            if (input == "n" || input == "N")
            {
                Environment.Exit( 0 );
            }

            System.IO.Directory.CreateDirectory( animeDirectory );
        }

        try
        {
            var allFiles = from file in System.IO.Directory.EnumerateFiles( System.IO.Directory.GetCurrentDirectory( ), "[*- *", System.IO.SearchOption.AllDirectories )
                           select file;
            int fileCount = allFiles.Count( );
            Console.Write( "Identified {0} files to be moved, continue? Y/N ", fileCount );
            input = Console.ReadLine( );
            while (!(validInputs.Contains( input )))
            {
                Console.WriteLine( "Not a valid input." );
                input = Console.ReadLine( );
            }
            
            if (input == "n" || input == "N")
            {
                Environment.Exit( 0 );
            }

            foreach (var file in allFiles)
            {
                String fileName = System.IO.Path.GetFileName( file );
                String Title;
                String Episode;

                animeProcess( fileName, out Title, out Episode );
                String destinationDirectory = animeDirectory + "\\" + Title;
                if (!(System.IO.Directory.Exists( destinationDirectory )))
                {
                    System.IO.Directory.CreateDirectory( destinationDirectory );
                    Console.WriteLine( "Created {0}", destinationDirectory );
                }
                String destinationPath = destinationDirectory + "\\" + Title + " - " + Episode + System.IO.Path.GetExtension( file );
                System.IO.File.Move( @file, @destinationPath );
            }
        }
        catch (Exception e)
        {
            Console.WriteLine( );
            Console.WriteLine( "It dun work." );
            Console.WriteLine( e.ToString( ) );
        }
    }
}


