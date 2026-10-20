using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace ScrewPuzzle.Tests
{
    public class WorkshopShaderTests
    {
        [Test]
        public void RuntimeToyShader_IsIncludedInPlayerBuilds()
        {
            var shader = Shader.Find("Standard");
            Assert.That(shader, Is.Not.Null);
            var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            var included = settings.FindProperty("m_AlwaysIncludedShaders");
            bool found = false;
            for (int i = 0; i < included.arraySize; i++)
                found |= included.GetArrayElementAtIndex(i).objectReferenceValue == shader;
            Assert.That(found, Is.True, "Procedural toy materials need Standard preserved in Android builds.");
        }
    }
}
