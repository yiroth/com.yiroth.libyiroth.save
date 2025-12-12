using UnityEngine;
using LibYiroth;

namespace LibYiroth.Save
{
    public class SaveManager : MonoBehaviour
    {
        private GameSlot _activeGameSlot;

        private void Start()
        {
            _activeGameSlot = new GameSlot();
        }

        public bool SaveGameSlot(string slotName)
        {
            return false;
        }

        public GameSlot GetActiveGameSlot()
        {
            return _activeGameSlot;
        }

        public bool SaveVariable<T>(Data.Identification id, string variableName, T value)
        {
            if (_activeGameSlot == null)
                return false;
            
            if(!id.IsValid())
            {
                Debug.LogError("SaveVariable: Level ID is not valid!");
                return false;
            }

            if(string.IsNullOrEmpty(variableName))
            {
                Debug.LogError("SaveVariable: VariableName is null or empty!");
                return false;
            }
            
            // Create a new save variable container
            SavedVariable sv = new SavedVariable(id, variableName, new Variant.Variant(value));
            
            // Create a key for the save variable container
            SaveKey key = new SaveKey(ref sv.id, ref sv.variableName);
            
            // Add them to the slot
            _activeGameSlot.SavedVariables.Add(key, sv);
            _activeGameSlot.UniqueIdentifications.Add(sv.id);

            return true;
        }
    }
}