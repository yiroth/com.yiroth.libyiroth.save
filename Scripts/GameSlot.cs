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

using System.Collections.Generic;
using LibYiroth;

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

        public SaveKey(ref Data.Identification id, ref string variableName)
        {
            _id = id;
            _variableName = variableName;
        }

        public bool Equals(SaveKey other)
        {
            return this._id.Equals(other._id);
        }
    }
    
    [System.Serializable]
    public struct SavedVariable : System.IEquatable<SavedVariable>
    {
        public Data.Identification id;
        public string variableName;
        public Variant.Variant variable;

        public SavedVariable(Data.Identification id, string variableName, Variant.Variant variable)
        {
            this.id = id;
            this.variableName = variableName;
            this.variable = variable;
        }

        public bool Equals(SavedVariable other)
        {
            return this.id.Equals(other.id);
        }
    }
    
    public class GameSlot
    {
        public string SlotName = string.Empty;
        public Data.Date SaveDate;
        public Data.Time SaveTime;
        public int SlotVersion = 1;
        public SortedSet<Data.Identification> UniqueIdentifications;
        public Dictionary<SaveKey, SavedVariable> SavedVariables;

        public GameSlot()
        {
            SavedVariables = new Dictionary<SaveKey, SavedVariable>();
        }

        public Dictionary<SaveKey, SavedVariable> GetSavedSceneObjects()
        {
            return SavedVariables;
        }
    }
}