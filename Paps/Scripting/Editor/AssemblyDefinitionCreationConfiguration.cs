using UnityEditorInternal;
using UnityEngine;

namespace Paps.Scripting.Editor
{
    public class AssemblyDefinitionCreationConfiguration : ScriptableObject
    {
        [field: SerializeField] public AssemblyDefinitionAsset[] DefaultAssemblyDefinitions { get; private set; }
    }
}