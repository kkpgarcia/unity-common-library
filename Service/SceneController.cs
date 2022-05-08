using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Common.Events;

namespace Common.Services
{
	public class SceneController
	{
		public const string OnSceneProcessStartEvent = "SceneController.OnSceneLoadStart";
		public const string OnSceneProgressUpdateEvent = "SceneController.OnSceneLoadProgressUpdate";
		public const string OnSceneProcessCompleteEvent = "SceneController.OnSceneLoadComplete";
		private string _currentLoadedScene;

		public Scene GetCurrentScene()
		{
			return SceneManager.GetActiveScene();
		}

		public void MoveObjectToScene(GameObject obj, string sceneName)
		{
			SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByName(sceneName));
		}

		public async void LoadScene(string name)
		{
			this.PostNotification(OnSceneProcessStartEvent);

			_currentLoadedScene = name;

			await ProcessSceneOperation(SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive));
		}

		public async void UnloadScene(string name)
		{
			this.PostNotification(OnSceneProcessStartEvent);
			await ProcessSceneOperation(SceneManager.UnloadSceneAsync(name));
		}

		private async Task ProcessSceneOperation(AsyncOperation operation)
		{
			operation.completed += OnSceneLoadCompleteHandler;

			while (!operation.isDone)
			{
				float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
				this.PostNotification(OnSceneProgressUpdateEvent, new InfoEventArgs<float>(progressValue));
				await Task.Yield();
			}
		}

		private void OnSceneLoadCompleteHandler(AsyncOperation operation)
		{
			Scene loadedScene = SceneManager.GetSceneByName(_currentLoadedScene);
			if (!string.IsNullOrEmpty(_currentLoadedScene))
			{
				if (loadedScene.isLoaded)
				{
					SceneManager.SetActiveScene(loadedScene);
				}

				Clean();
			}

			this.PostNotification(OnSceneProgressUpdateEvent, new InfoEventArgs<float>(1));
			this.PostNotification(OnSceneProcessCompleteEvent, new InfoEventArgs<Scene>(loadedScene));
		}

		public void SetActiveScene(string sceneName)
		{
			Scene toActiveScene = SceneManager.GetSceneByName(sceneName);
			SceneManager.SetActiveScene(toActiveScene);
		}

		private void Clean()
		{
			_currentLoadedScene = "";
		}
	}
}
