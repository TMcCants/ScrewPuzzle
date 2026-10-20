using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ScrewPuzzle.Tests
{
    public sealed class TrainPresentationTests : WorkshopRunTestIsolation
    {
        [UnityTest]
        public IEnumerator LocksExplainBlockers_LidsRequirePreviewConfirmation_AndStatesRestore()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Selector);
            yield return new EnterPlayMode(); yield return null;
            ThreeDBoardNavigation.OpenToyTrain(); yield return null; yield return null;
            var input=Object.FindFirstObjectByType<Radio3DInteraction>();
            Assert.That(input,Is.Not.Null,"Train interaction must be initialized");
            var puzzle=input.GetComponent<Radio3DPuzzle>();
            Assert.That(input.Radio != null,Is.True,"Train model must be initialized");
            var cab=input.Radio.GetComponentsInChildren<Radio3DScrew>(true).Single(s=>s.name=="Cab 0");
            var headTransform=cab.transform.Find("Head Visual");
            Assert.That(headTransform,Is.Not.Null,"Cab head should exist");
            var head=headTransform.GetComponent<Renderer>();
            var properties=new MaterialPropertyBlock(); head.GetPropertyBlock(properties);
            Assert.That(properties.GetColor("_Color"),Is.Not.EqualTo(head.sharedMaterial.color));
            input.Radio.rotation=Quaternion.FromToRotation(Vector3.up,Vector3.back);
            yield return null; Physics.SyncTransforms();
            Vector2 point=input.View.WorldToScreenPoint(cab.transform.position);
            Assert.That(input.PickVisibleScrew(point),Is.EqualTo(cab));
            Assert.That(input.PickScrew(point),Is.Null);
            Assert.That(GameObject.Find("Lock Marker Cab 0"),Is.Not.Null);
            input.BeginPointer(point); input.EndPointer(point);
            Assert.That(puzzle.RemovedCount,Is.Zero);
            Assert.That(puzzle.Message,Does.Contain("drive assembly"));
            Assert.That(WorkshopRunSave.HasRun(ThreeDBoardProgress.ToyTrain),Is.False);
            var unmatched=FindUnmatched(input,puzzle);
            input.Radio.rotation=Quaternion.FromToRotation(unmatched.transform.localRotation*Vector3.up,Vector3.back);
            yield return null; Physics.SyncTransforms();
            point=input.View.WorldToScreenPoint(unmatched.transform.position);
            input.BeginPointer(point);input.EndPointer(point);
            Assert.That(puzzle.Message,Does.StartWith("Finish an open tray"));
            Assert.That(puzzle.RemovedCount,Is.Zero);
            var availableHead=unmatched.transform.Find("Head Visual").GetComponent<Renderer>(); availableHead.GetPropertyBlock(properties);
            Assert.That(properties.GetColor("_Color"),Is.EqualTo(availableHead.sharedMaterial.color));
            var lid=GameObject.Find("Tray Lid 2");
            GameObject.Find("Unlock Tray 2").GetComponent<Button>().onClick.Invoke();
            Assert.That(puzzle.Trays[2].IsOpen,Is.False);
            Assert.That(GameObject.Find("Bonus Tray Preview"),Is.Not.Null);
            GameObject.Find("Cancel Unlock").GetComponent<Button>().onClick.Invoke();
            Assert.That(puzzle.Trays[2].IsOpen,Is.False);
            GameObject.Find("Unlock Tray 2").GetComponent<Button>().onClick.Invoke();
            GameObject.Find("Preview Open").GetComponent<Button>().onClick.Invoke();
            Assert.That(puzzle.Trays[2].IsOpen,Is.True);
            Assert.That(puzzle.Trays[3].IsOpen,Is.False);
            float lidDeadline=Time.realtimeSinceStartup+3f;
            while(lid.activeSelf && Time.realtimeSinceStartup<lidDeadline)yield return null;
            Assert.That(lid.activeSelf,Is.False);
            ThreeDBoardNavigation.OpenToyTrain();yield return null;yield return null;
            input=Object.FindFirstObjectByType<Radio3DInteraction>();puzzle=input.GetComponent<Radio3DPuzzle>();
            Assert.That(puzzle.Trays[2].IsOpen,Is.True);
            Assert.That(GameObject.Find("Tray Lid 2"),Is.Null,"Resume must keep an unlocked lid open");
            input.ResetExperiment();yield return null;
            Assert.That(puzzle.Trays[2].IsOpen,Is.False);
            Assert.That(GameObject.Find("Tray Lid 2"),Is.Not.Null);
            var layout=Object.FindFirstObjectByType<ToyTrain3DPrototypeBootstrap>().Layout;
            var screws=input.Radio.GetComponentsInChildren<Radio3DScrew>();
            for(int i=0;i<18;i++)
            {
                Assert.That(puzzle.TryAdd(screws[layout.Solution[i]]),Is.True);
                yield return ToyTrain3DTests.Wait(puzzle);
            }
            yield return null;
            cab=screws[18];head=cab.transform.Find("Head Visual").GetComponent<Renderer>();head.GetPropertyBlock(properties);
            Assert.That(cab.IsAccessible,Is.True);
            Assert.That(properties.GetColor("_Color"),Is.EqualTo(head.sharedMaterial.color));
            Assert.That(GameObject.Find("Lock Marker "+cab.name),Is.Null);
            for(int i=18;i<21;i++)
            {
                Assert.That(puzzle.TryAdd(screws[layout.Solution[i]]),Is.True);
                yield return ToyTrain3DTests.Wait(puzzle);
            }
            yield return null;
            Assert.That(puzzle.HasQueuedTrays,Is.False);
            var unneeded=GameObject.Find("Unlock Tray 2").GetComponent<Button>();
            Assert.That(unneeded.interactable,Is.False);
            Assert.That(unneeded.GetComponentInChildren<Text>().text,Is.EqualTo("NOT NEEDED"));
            unneeded.onClick.Invoke();
            Assert.That(GameObject.Find("Bonus Tray Preview"),Is.Null);
            input.ResetExperiment();yield return null;
            Assert.That(cab.IsAccessible,Is.False);
            head.GetPropertyBlock(properties);
            Assert.That(properties.GetColor("_Color"),Is.Not.EqualTo(head.sharedMaterial.color));
        }
        private static Radio3DScrew FindUnmatched(Radio3DInteraction input,Radio3DPuzzle puzzle)
        {
            foreach(var screw in input.Radio.GetComponentsInChildren<Radio3DScrew>())
            {
                bool matched=false;
                foreach(var tray in puzzle.Trays) if(tray.IsOpen && tray.HasColor && tray.Color==screw.ColorId)matched=true;
                if(screw.IsAccessible && !matched)return screw;
            }
            Assert.Fail("Expected an accessible screw whose color has no open tray");
            return null;
        }
        [UnityTearDown] public IEnumerator LeavePlayMode(){if(Application.isPlaying)yield return new ExitPlayMode();}
    }
}
