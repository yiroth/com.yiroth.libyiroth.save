/*
 * Copyright 2025 yiroth
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Purpose: A container for storing game data
 */

using System;
using System.Collections.Generic;
using LibYiroth;
using UnityEngine;

namespace LibYiroth.Save
{
    [System.Serializable]
    public struct SaveKey : System.IEquatable<SaveKey>
    {
        private Data.Identification _id;
        private string _variableName;

        public Data.Identification GetId()
        {
            return _id;
        }

        public SaveKey(Data.Identification id, string variableName)
        {
            _id = id;
            _variableName = variableName;
        }

        public bool Equals(SaveKey other)
        {
            return _id.Equals(other._id) && _variableName == other._variableName;
        }

        public override bool Equals(object obj)
        {
            return obj is SaveKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_id, _variableName);
        }

        public static bool operator ==(SaveKey left, SaveKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveKey left, SaveKey right)
        {
            return !left.Equals(right);
        }
    }
    
    [System.Serializable]
    public struct SavedVariable : System.IEquatable<SavedVariable>
    {
        public Data.Identification id;
        public string variableName;
        public Variant.Container container;

        public SavedVariable(Data.Identification id, string variableName, Variant.Container container)
        {
            this.id = id;
            this.variableName = variableName;
            this.container = container;
        }

        public bool Equals(SavedVariable other)
        {
            return id.Equals(other.id) && variableName == other.variableName && Equals(container, other.container);
        }

        public override bool Equals(object obj)
        {
            return obj is SavedVariable other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, variableName, container);
        }

        public static bool operator ==(SavedVariable left, SavedVariable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SavedVariable left, SavedVariable right)
        {
            return !left.Equals(right);
        }
    }
    
    [System.Serializable]
    public class GameSlot : ISerializationCallbackReceiver
    {
        [SerializeField] private int _slotID = 0;
        [SerializeField] private string _prettyName;
        [SerializeField] private Data.Date _saveDate;
        [SerializeField] private Data.Time _saveTime;
        [SerializeField] private int _slotVersion;
        
        // Serialized form (JsonUtility-friendly):
        [SerializeField] private List<SavedVariable> _savedVariablesList = new List<SavedVariable>();

        // Runtime form (fast lookup):
        [NonSerialized] private Dictionary<SaveKey, SavedVariable> _savedVariables;


        public GameSlot(int id, int version)
        {
            _slotID = id;
            _slotVersion = version;

            _savedVariables = new Dictionary<SaveKey, SavedVariable>();
            _savedVariablesList = new List<SavedVariable>();
        }
        
        public void OnBeforeSerialize()
        {
            // Build list from dictionary
            _savedVariablesList ??= new List<SavedVariable>();
            _savedVariablesList.Clear();

            if (_savedVariables == null)
                return;

            foreach (var pair in _savedVariables)
                _savedVariablesList.Add(pair.Value);
        }

        public void OnAfterDeserialize()
        {
            // Build dictionary from list
            _savedVariables = new Dictionary<SaveKey, SavedVariable>();

            if (_savedVariablesList == null)
                return;

            foreach (var v in _savedVariablesList)
            {
                var key = new SaveKey(v.id, v.variableName);
                if (!_savedVariables.TryAdd(key, v))
                {
                    // optional: log warning and keep the first value
                    Debug.Log("Duplicate key: " + key + " found");
                }
            }
        }

        public int GetSlotID()
        {
            return _slotID;
        }

        public void SetPrettyName(string prettyName)
        {
            _prettyName = prettyName;
        }

        public void SetSaveDate(Data.Date saveDate)
        {
            _saveDate = saveDate;
        }

        public void SetSaveTime(Data.Time saveTime)
        {
            _saveTime = saveTime;
        }

        public int GetVersion()
        {
            return _slotVersion;
        }

        public bool UpdateVersion(int newVersion)
        {
            if (_slotVersion != newVersion && newVersion > _slotVersion)
            {
                // TODO: do logical update here
                return true;
            }

            return false;
        }

        public void AddSavedVariable(SaveKey key, SavedVariable savedVariable)
        {
            _savedVariables ??= new Dictionary<SaveKey, SavedVariable>();
            _savedVariables[key] = savedVariable;
        }

        public Dictionary<SaveKey, SavedVariable> GetSavedVariables()
        {
            _savedVariables ??= new Dictionary<SaveKey, SavedVariable>();
            return _savedVariables;
        }

        public GameSlot Snapshot()
        {
            GameSlot save = new GameSlot(_slotID, _slotVersion);

            if (Helper.Cardinal.IsNotValid(_savedVariables)) return save;
            
            foreach (var pair in _savedVariables)
            {
                var v = pair.Value;
                var containerCopy = Helper.Cardinal.IsValid(v.container) ? v.container.Clone() : null;
                save._savedVariables[pair.Key] = new SavedVariable(v.id, v.variableName, containerCopy);
            }

            return save;
        }
    }
}