using HLSTools.NETFramework;
using System;
using System.Collections.Generic;
using System.IO;

namespace FFmePlayer_snu.Controls
{
    public class MediaSegmentBuffer : IDisposable
    {
        private const string MediaSegmentContentFilename = "media{0}.ts";

        /// <summary>
        /// Fired, when a media segment is added into this buffer.
        /// Will provide the number of media segments in the buffer.
        /// </summary>
        public event EventHandler<int> BufferChanged;

        public int CurrentIndex
        {
            get;
            set;
        }


        private IList<MediaSegmentContent> _mediaSegmentContentList;

        public MediaSegmentBuffer()
        {
            _mediaSegmentContentList = new List<MediaSegmentContent>();
        }

        /// <summary>
        /// Adds the given media into the buffer.
        /// </summary>
        /// <param name="mediaSegmentContent">The media to add.</param>
        public void Add(MediaSegmentContent mediaSegmentContent)
        {
            if(mediaSegmentContent != null)
            {
                FileWriter.WriteBytesToFile(
                    mediaSegmentContent.FullContent,
                    string.Format(MediaSegmentContentFilename, _mediaSegmentContentList.Count));

                _mediaSegmentContentList.Add(mediaSegmentContent);

                System.Diagnostics.Debug.WriteLine($"Media segment number {_mediaSegmentContentList.Count} added to buffer");

                BufferChanged?.Invoke(this, _mediaSegmentContentList.Count);
            }
        }


        /// <summary>
        /// Resolves the next media and the associated .ts file that is stored locally.
        /// </summary>
        /// <param name="mediaSegmentContent">The next media.</param>
        /// <param name="localTsFileUri">The .ts file associated with the media segment.</param>
        /// <returns>True, if the method was able to resolve the next media. False otherwise.</returns>
        public bool TryGetNext(out MediaSegmentContent mediaSegmentContent, out Uri localTsFileUri)
        {
            if (_mediaSegmentContentList.Count > CurrentIndex)
            {
                mediaSegmentContent = _mediaSegmentContentList[CurrentIndex];
                string filename = string.Format(MediaSegmentContentFilename, CurrentIndex);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
                localTsFileUri = new Uri(filePath);
                CurrentIndex++;
                return true;
            }

            mediaSegmentContent = null;
            localTsFileUri = null;
            return false;
        }

        public bool TryGetPrevious(out MediaSegmentContent mediaSegmentContent, out Uri localTsFileUri)
        {
            if (_mediaSegmentContentList.Count > CurrentIndex)
            {
                mediaSegmentContent = _mediaSegmentContentList[CurrentIndex];
                string filename = string.Format(MediaSegmentContentFilename, CurrentIndex);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
                localTsFileUri = new Uri(filePath);
                CurrentIndex--;
                return true;
            }

            mediaSegmentContent = null;
            localTsFileUri = null;
            return false;
        }

        public bool PlayFirst(out MediaSegmentContent mediaSegmentContent, out Uri localTsFileUri)
        {
            if (_mediaSegmentContentList.Count > CurrentIndex)
            {
                CurrentIndex = 0;
                mediaSegmentContent = _mediaSegmentContentList[CurrentIndex];
                string filename = string.Format(MediaSegmentContentFilename, CurrentIndex);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
                localTsFileUri = new Uri(filePath);
                return true;
            }

            mediaSegmentContent = null;
            localTsFileUri = null;
            return false;
        }

        public void Dispose(int index)
        {


            // TODO: Delete TS file
            string filename = string.Format(MediaSegmentContentFilename, index - 1);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
            System.IO.FileInfo fi = new System.IO.FileInfo(filePath);
            try
            {
                if(index >= 1)
                fi.Delete();
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Dispose()
        {
            //foreach (MediaSegmentContent mediaSegmentContent in _mediaSegmentContentList)
            //{
            //    mediaSegmentContent.Dispose();
            //}

            //_mediaSegmentContentList.Clear();

            // TODO: Delete TS files

            int index = 0;
            while (index >= 0)
            {
                if (_mediaSegmentContentList.Count > index
                    && _mediaSegmentContentList[index] != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Disposing buffered media segment with index {index}");
                    _mediaSegmentContentList[index].Dispose();
                    _mediaSegmentContentList[index] = null;
                }


                string filename = string.Format(MediaSegmentContentFilename, index);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
                System.IO.FileInfo fi = new System.IO.FileInfo(filePath);
                try
                {
                    if (fi.Exists)
                        fi.Delete();
                    else
                    {
                        break;
                    }
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                }
                index++;
            }

        }
    }
}
