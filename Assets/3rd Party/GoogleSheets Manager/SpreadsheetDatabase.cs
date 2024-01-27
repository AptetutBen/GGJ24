using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleSheetManager
{

	[System.Serializable]
	public class SpreadsheetDatabase : ScriptableObject
	{
		public string DatabaseName;
		public string[] Headers;
		public SpreadsheetDataSet[] DataSets;

		//Load data into the database
		public void LoadData(List<List<string>> dataArray, string databaseName)
		{
			//First row is headers
			Headers = dataArray[0].ToArray();
			DatabaseName = databaseName;
			DataSets = new SpreadsheetDataSet[dataArray.Count - 1];

			//All other rows get turned into datasets
			for (int i = 1; i < dataArray.Count; i++)
			{
				SpreadsheetDataSet newDataSet = new SpreadsheetDataSet(dataArray[i].ToArray(), this);
				DataSets[i-1] = newDataSet;
			}
		}

		//Get a given dataset via the first column
		public SpreadsheetDataSet FindByKey(string keyValue)
		{
			return Array.Find(DataSets, d => d.Key == keyValue);
		}

        void OnValidate() {

            for (int i = 0; i < DataSets.Length; i++) {
                if (DataSets[ i ].Database == null) { DataSets[ i ].Database = this; }
            }
        }
    }

	[System.Serializable]
	public class SpreadsheetDataSet
	{
		//Store the keys and value seperate for ease of serialisation, linked by index
        public string[] Values;
        
        [HideInInspector] [SerializeField] internal SpreadsheetDatabase Database;

		//Get the value stored in the First Column
		public string Key
		{
			get { return Values[0]; }
		}

		//Constructor
		public SpreadsheetDataSet(string[] values, SpreadsheetDatabase database)
		{
			Values        = values;
            this.Database = database;
        }

		//Get a value from the given key
		public string GetValue(string key)
		{
			int position = Array.IndexOf(Database.Headers, key);
			if (position == -1)
			{
				//Debug.LogError("Dataset does not contain key: " + key);
				return "";
			}

			return Values[position];
		}

		//Yes this is a passthrough function, so shoot me
		public string GetValueAsString(string key)
		{
			return GetValue(key);
		}

		//Return the value as a float
		public float GetValueAsFloat(string key)
		{
			float result;
			string value = GetValue(key);
			if (float.TryParse(value, out result))
			{
				return result;
			}
			else
			{
				Debug.LogError("Could not parse :" + value + " as a float");
				return 0;

			}
		}

		//Return the value as a bool
		public bool GetValueAsBool(string key)
		{
			string value = GetValue(key);
			if (value.ToUpper() == "TRUE")
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		//Return the value as an Int
		public int GetValueAsInt(string key)
		{
			int result;
			string value = GetValue(key);
			if (int.TryParse(value, out result))
			{
				return result;
			}
			else
			{
				//Debug.LogError("Could not parse :" + value + " as a int");
				return 0;
			}
		}

		public string[] GetValueAsStringArray(string key, char seperatorChar = ',')
		{
			string[] result;
			string value = GetValue(key);

			result = value.Split(seperatorChar);

			return result;
		}

		public int[] GetValueAsIntArray(string key, char seperatorChar = ',')
		{
			string[] result = GetValueAsStringArray(key, seperatorChar);
			int[] resultsInt = Array.ConvertAll(result, s => int.Parse(s));
			return resultsInt;
		}

		public float[] GetValueAsFloatArray(string key, char seperatorChar = ',')
		{
			string[] result = GetValueAsStringArray(key, seperatorChar);
			float[] resultsFloat = Array.ConvertAll(result, s => float.Parse(s));
			return resultsFloat;
		}

		public bool[] GetValueAsBoolArray(string key, char seperatorChar = ',')
		{
			string[] result = GetValueAsStringArray(key, seperatorChar);
			bool[] resultsFloat = Array.ConvertAll(result, s => bool.Parse(s));
			return resultsFloat;
		}
	}
}