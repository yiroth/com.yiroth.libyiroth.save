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
 * Purpose: Manager component for saving/loading the game and storing/processing game slots
 */

using System;
using UnityEngine;
using LibYiroth;

namespace LibYiroth.Save
{
    public class SaveManager : MonoBehaviour
    {
        public int CurrentVersion = 1;
        
        private GameSlot _activeGameSlot;

        private void Start()
        {
            // TODO: Check for a new game
                // If it is, then create a new game slot

                int nextId = 0;
                if (FindNextId(out nextId))
                {
                    _activeGameSlot = new GameSlot(nextId, CurrentVersion);
                    
                }
        }
        
        public bool SaveVariable<T>(Data.Identification ownerId, string variableName, T variableValue, bool overwrite = true)
        {
            if (_activeGameSlot == null)
                return false;

            if (string.IsNullOrWhiteSpace(variableName))
                return false;

            SaveKey key = new SaveKey(ownerId, variableName);

            var vars = _activeGameSlot.GetSavedVariables();

            if (!overwrite && vars.ContainsKey(key))
                return false;

            Variant.Variant variant = new Variant.Variant(variableValue);
            Variant.Container container = new Variant.Container(variableName, variant.GetType(), variant);

            SavedVariable variable = new SavedVariable(ownerId, variableName, container);

            _activeGameSlot.AddSavedVariable(key, variable);
            return true;
        }

        public bool LoadVariable<T>(Data.Identification ownerId, string variableName, ref T variableValue)
        {
            variableValue = default;

            if (_activeGameSlot?.GetSavedVariables() == null)
                return false;

            SaveKey key = new SaveKey(ownerId, variableName);

            if (!_activeGameSlot.GetSavedVariables().TryGetValue(key, out SavedVariable saved))
                return false;

            Variant.VariantTypes expectedType = Variant.Variant.FindVariantType<T>();
            if (expectedType == Variant.VariantTypes.Empty)
                return false;

            if (saved.container.GetVariableType() != expectedType)
                return false;

            variableValue = saved.container.GetVariable<T>();
            return true;
        }

        public bool SaveGameToFile(string prettyName = null)
        {
            // TODO: Warn the objects, Do directory and file search, JSONize the slot, fill other information, save to a file
            if (_activeGameSlot == null)
                return false;

            GameSlot save = _activeGameSlot.Snapshot();

            if (!string.IsNullOrEmpty(prettyName))
                save.SetPrettyName(prettyName);

            save.SetSaveDate(new Data.Date(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            save.SetSaveTime(new Data.Time(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

            string jsonify = JsonUtility.ToJson(save, true);
            
            // TODO: Write to a file
            
            return false;
        }

        public bool FindNextId(out int found)
        {
            found = 0;
            return true;
        }

        public bool LoadGameFromFile(int id)
        {
            string jsonify = System.IO.File.ReadAllText("");
            GameSlot loaded = JsonUtility.FromJson<GameSlot>(jsonify);
            // TODO: Do directory and file search, read it as a JSON, warn the objects
            return false;
        }

        public GameSlot GetCurrentGameSlot()
        {
            return _activeGameSlot;
        }

        public bool IsCurrentGameSlotActive()
        {
            return Helper.Cardinal.IsValid(_activeGameSlot);
        }
    }
}