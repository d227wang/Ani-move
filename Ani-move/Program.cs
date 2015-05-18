using System;
using System.Collections.Generic;
using System.Linq;


class Animove
{
    static void log(string logMessage, int warnLevel = 0)
    {
        // warnLevel denotes the "type" of log message to be displayed, options are as follows:
        // 0 = Green; stuff's working
        // 1 = Yellow; Encountered a recoverable exception
        // 2 = Red; Unrecoverable exception, unhandled exception
        // For simplicity's sake, this is an optional parameter, with 0 (green) being the default if nothing is passed

        // header that will be displayed in front of every log
        string header = "[Ani-move] " + DateTime.Now.ToString( "h:mm" ) + " | ";

        if (warnLevel == 2)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine( header + logMessage );
            Console.ResetColor( );
        }

        else if (warnLevel == 1)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine( header + logMessage );
            Console.ResetColor( );
        }

        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine( header + logMessage );
            Console.ResetColor( );
        }
        
    }

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
        Console.WriteLine( "Build 1.0.2");
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

            // Will display a message if no files are found, and then exit
            if(fileCount == 0)
            {
                log("No anime files found! Maybe wrong directory?", 1 );
                Console.ReadKey( );
                Environment.Exit( 0 );
            }

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

            log( string.Format( "Beginning main batch process on {0} files", fileCount ));

            foreach (var file in allFiles)
            {
                String fileName = System.IO.Path.GetFileName( file );
                String Title;
                String Episode;
                animeProcess( fileName, out Title, out Episode );
                log( string.Format( "Working on {0} - {1}", Title, Episode ) );
                String destinationDirectory = animeDirectory + "\\" + Title;

                if (!(System.IO.Directory.Exists( destinationDirectory )))
                {
                    log( string.Format( "Discovered that {0} doesn't exist. Creating", destinationDirectory ) );
                    System.IO.Directory.CreateDirectory( destinationDirectory );
                    log(string.Format("Created {0}", destinationDirectory));
                }

                String destinationPath = destinationDirectory + "\\" + Title + " - " + Episode + System.IO.Path.GetExtension( file );
                log( string.Format( "Moving {0} - {1}...", Title, Episode ) );
                System.IO.File.Move( @file, @destinationPath );
                log(string.Format("Successfully moved {0} - {1}", Title, Episode));

            }

            log( string.Format( "Successfully moved {0} files. Press any key to exit", fileCount ) );
            Console.ReadKey( );
        }
        catch (Exception e)
        {
            log("Unhandled exception occured! Press any key to exit");
            log( e.ToString( ) , 2);
            Console.ReadKey( );
        }
    }
}