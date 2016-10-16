import System.IO.File;
import System.IO.Stream;
import ICSharpCode.SharpZipLib.Core;
import ICSharpCode.SharpZipLib.Zip;

// this script unzips a container at a given path and returns a input stream to a given file in this container
public static function Unzip(path : String, fileName : String)
{
    var fileStream = OpenRead(path);
    var zipFile = new ZipFile(fileStream);

    for (var zipEntry : ZipEntry  in zipFile)
    {
        if (zipEntry.Name == fileName)
        {
            return zipFile.GetInputStream(zipEntry);
        }
    }

    return;
}
