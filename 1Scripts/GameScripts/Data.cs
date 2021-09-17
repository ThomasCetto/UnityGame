using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NoNameGame
{
    public class Data : MonoBehaviour
    {
        public static void SaveProfile(ProfileData profile)
        {
            try
            {
                string path = Application.persistentDataPath + "/profile.dt";

                if (File.Exists(path)) File.Delete(path);

                FileStream file = File.Create(path);

                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, profile);
                file.Close();
            }
            catch
            {
                Debug.Log("ERROR SAVING");
                //Debug.Log(e.ToString());
            }


        }

        public static ProfileData LoadProfile()
        {
            ProfileData ret = new ProfileData();

            try
            {
                string path = Application.persistentDataPath + "/profile.dt";


                if (File.Exists(path))
                {
                    FileStream file = File.Open(path, FileMode.Open);
                    BinaryFormatter bf = new BinaryFormatter();
                    ret = (ProfileData)bf.Deserialize(file);
                    file.Close();
                }
            }
            catch
            {
                Debug.Log("ERROR LOADING");
            }


            return ret;
        }






    }
}