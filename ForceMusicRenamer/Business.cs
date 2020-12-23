using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = TagLib.File;

namespace ForceMusicRenamer {
	internal class Business {
		internal static string RenameAllMusicByPath(string path, bool includeSubfolders) {
			var musicDirectory = new DirectoryInfo(path);
			if (!musicDirectory.Exists) {
				return "Status: Error! Directory does not exist!";
			}

			FileInfo[] files = musicDirectory.GetFiles("*", includeSubfolders
				                                                ? SearchOption.AllDirectories
				                                                : SearchOption.TopDirectoryOnly);
			if (files.Length == 0) {
				return "Status: Error! No any files in directory!";
			}

			var filesToRename = new List<KeyValuePair<FileInfo, string>>();
			string unknownArtist = "Unknown artist";
			string unknownTitle = "Unknown track";
			int unknownTrackNumber = 0;

			foreach (FileInfo file in files) {
				var song = File.Create(file.FullName);
				string artist = song.Tag.FirstPerformer;
				string title = song.Tag.Title;

				if ((artist != null) && (title != null)
				                     && (!file.Name.Contains(artist) || !file.Name.Contains(title))) {
					filesToRename.Add(new KeyValuePair<FileInfo, string>(file,
					                                                     GenerateFullName(path, artist, title, file.Extension)));
				} else if ((artist == null) && (title != null)
				                            && !file.Name.Contains(title)) {
					filesToRename.Add(new KeyValuePair<FileInfo, string>(file,
					                                                     GenerateFullName(path, unknownArtist, title, file.Extension)));
				} else if ((artist != null) && (title == null)
				                            && !file.Name.Contains(artist)) {
					unknownTrackNumber++;
					filesToRename.Add(new KeyValuePair<FileInfo, string>(file,
					                                                     GenerateFullName(path, artist, unknownTitle, file.Extension, unknownTrackNumber)));
				} else if ((artist == null) && (title == null)
				                            && (!file.Name.Contains(unknownArtist) || !file.Name.Contains(unknownTitle))) {
					unknownTrackNumber++;
					filesToRename.Add(new KeyValuePair<FileInfo, string>(file,
					                                                     GenerateFullName(path, unknownArtist, unknownTitle, file.Extension, unknownTrackNumber)));
				}
			}

			foreach (KeyValuePair<FileInfo, string> fileToRename in filesToRename) {
				fileToRename.Key.CopyTo(fileToRename.Value);
				fileToRename.Key.Delete();
			}
			return $"Status: Completed. {filesToRename.Count} files renamed.";
		}

		private static string GenerateFullName(string path, string artist, string title, string extension, int unknownTrackNumber = 0) {
			string fileName = unknownTrackNumber != 0
				                  ? $"{path}\\{artist} - {title} {unknownTrackNumber}{extension}"
				                  : $"{path}\\{artist} - {title}{extension}";
			return ReplaceIllegalCharactersToUnderscores(fileName);
		}

		private static string ReplaceIllegalCharactersToUnderscores(string fileName) => Path.GetInvalidFileNameChars()
		                                                                                    .Aggregate(fileName, (current, c) => current.Replace(c, '_'));
	}
}
