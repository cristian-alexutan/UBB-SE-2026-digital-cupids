using matchmaking.Repositories;
using matchmaking.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace matchmaking.Services
{
    internal class PhotoService
    {
        private PhotoRepository PhotoRepo;

        private static readonly string[] Extensions = { ".jpeg", ".jpg", ".png" };
        private const long MaxFileSize = 10 * 1024 * 1024;

        public PhotoService(PhotoRepository photoRepo)
        {
            PhotoRepo = photoRepo;
        }

        public void AddPhoto(Photo photo)
        {
            if (!File.Exists(photo.Location))
            {
                throw new FileNotFoundException("The file path to the photo does not exist!");
            }

            string extension = Path.GetExtension(photo.Location).ToLower();
            if (!Extensions.Contains(extension))
            {
                throw new Exception("Only JPEG and PNG files are allowed!");
            }

            long fileSize = new FileInfo(photo.Location).Length;
            if (fileSize > MaxFileSize)
            {
                throw new Exception("The file is too large! Maximum size is 10MB.");
            }

            var currentPhotos = GetPhotosByUserId(photo.UserId);
            if (currentPhotos.Count >= 6)
            {
                throw new Exception("You cannot upload more than 6 photos!");
            }

            string storageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StoredPhotos");
            Directory.CreateDirectory(storageDirectory);

            string fileName = Guid.NewGuid().ToString() + extension;
            string destinationPath = Path.Combine(storageDirectory, fileName);

            File.Copy(photo.Location, destinationPath);

            photo.Location = destinationPath;
            photo.ProfileOrderIndex = currentPhotos.Count;

            PhotoRepo.Add(photo);
        }

        public Photo DeleteById(int photoId)
        {
            Photo photo = FindById(photoId);

            List<Photo> userPhotos = GetPhotosByUserId(photo.UserId);
            if (userPhotos.Count <= 2)
            {
                throw new Exception("The photo cannot be deleted! You must have at least 2 remaining photos.");
            }

            string storageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StoredPhotos");
            if (File.Exists(photo.Location) && photo.Location.StartsWith(storageDirectory, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    File.Delete(photo.Location);
                }
                catch (Exception)
                {
                }
            }

            Photo? deletedPhoto = PhotoRepo.DeleteById(photoId);

            if (deletedPhoto == null)
            {
                throw new Exception("The photo cannot be deleted from the database!");
            }

            List<Photo> remainingPhotos = GetPhotosByUserId(deletedPhoto.UserId);
            for (int i = 0; i < remainingPhotos.Count; ++i)
            {
                remainingPhotos[i].ProfileOrderIndex = i;
                PhotoRepo.Update(remainingPhotos[i]);
            }

            return deletedPhoto;
        }

        public Photo FindById(int photoId)
        {
            Photo? photo = PhotoRepo.FindById(photoId);
            if (photo == null)
            {
                throw new Exception("The photo does not exist!");
            }
            return photo;
        }

        public List<Photo> GetPhotosByUserId(int userId)
        {
            return PhotoRepo.GetAll()
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.ProfileOrderIndex)
                .ToList();
        }

        public List<Photo> DeletePhotosByUserId(int userId)
        {
            List<Photo> userPhotos = GetPhotosByUserId(userId);

            foreach (Photo photo in userPhotos)
            {
                if (File.Exists(photo.Location))
                {
                    File.Delete(photo.Location);
                }
                PhotoRepo.DeleteById(photo.PhotoId);
            }

            return userPhotos;
        }

        public void ReorderPhotos(int userId, List<int> newPhotoIds)
        {
            for (int i = 0; i < newPhotoIds.Count; ++i)
            {
                int photoId = newPhotoIds[i];
                Photo? photo = PhotoRepo.FindById(photoId);

                if (photo != null && photo.UserId == userId)
                {
                    photo.ProfileOrderIndex = i;
                    PhotoRepo.Update(photo);
                }
            }
        }
    }
}