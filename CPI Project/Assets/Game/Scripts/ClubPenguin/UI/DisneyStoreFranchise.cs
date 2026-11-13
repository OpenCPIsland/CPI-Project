using ClubPenguin.DisneyStore;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace ClubPenguin.UI
{
    public class DisneyStoreFranchise : MonoBehaviour
    {
        public Transform ItemContainer;
        public RectTransform ConfirmationContainer;
        public ScrollRect ContentScrollRect;
        public Image IconImage;
        public Image HeaderImage;
        public Image[] ImagesToTint;

        internal static readonly PrefabContentKey DisneyStoreItemPrefabKey = new PrefabContentKey("DisneyShop/Prefabs/FranchiseItemButton");
        internal static readonly PrefabContentKey PurchaseConfirmationPrefabKey = new PrefabContentKey("DisneyShop/Prefabs/ClothingPurchase");
        internal static readonly PrefabContentKey OwnedConfirmationPrefabKey = new PrefabContentKey("DisneyShop/Prefabs/ClothingOwned");
        internal static readonly PrefabContentKey ConsumableConfirmationPrefabKey = new PrefabContentKey("DisneyShop/Prefabs/PartySupplyPurchase");
        internal static readonly PrefabContentKey IglooConfirmationPrefabKey = new PrefabContentKey("DisneyShop/Prefabs/IglooItemPurchase");

        private DisneyStoreFranchiseDefinition franchiseDef;
        private IDisneyStoreController storeController;
        private GameObject itemPrefab;
        private GameObject confirmation;

        public void SetFranchise(DisneyStoreFranchiseDefinition franchiseDef, IDisneyStoreController storeController)
        {
            this.franchiseDef = franchiseDef;
            this.storeController = storeController;
            loadFranchise();
        }

        public void Clear()
        {
            HideConfirmation();
            int childCount = ItemContainer.childCount;
            for (int num = childCount - 1; num >= 0; num--)
            {
                UnityEngine.Object.Destroy(ItemContainer.GetChild(num).gameObject);
            }
            IconImage.sprite = null;
            HeaderImage.sprite = null;
        }

        private void loadFranchise()
        {
            Content.LoadAsync(onItemPrefabLoaded, DisneyStoreItemPrefabKey);
            if (!string.IsNullOrEmpty(franchiseDef.FranchiseIconPath.Key))
            {
                CoroutineRunner.Start(loadSprite(franchiseDef.FranchiseIconPath, IconImage), this, "");
            }
            if (!string.IsNullOrEmpty(franchiseDef.FranchiseHeaderPath.Key))
            {
                CoroutineRunner.Start(loadSprite(franchiseDef.FranchiseHeaderPath, HeaderImage), this, "");
            }
            applyImageTints();
        }

        private void applyImageTints()
        {
            for (int i = 0; i < ImagesToTint.Length; i++)
            {
                ImagesToTint[i].color = franchiseDef.FranchiseBackgroundColor;
            }
        }

        private void onItemPrefabLoaded(string path, GameObject itemPrefab)
        {
            this.itemPrefab = itemPrefab;
            createStoreItems();
        }

        private IEnumerator loadSprite(TypedAssetContentKey<Sprite> key, Image image)
        {
            if (image == null)
            {
                Debug.LogWarning("DisneyStoreFranchise cannot load sprite '" + key.Key + "' because its target Image is missing.", this);
                yield break;
            }
            AssetRequest<Sprite> request = null;
            try
            {
                request = Content.LoadAsync(key);
            }
            catch (Exception ex)
            {
                Log.LogException(this, ex);
            }
            if (request != null)
            {
                yield return request;
                if (image == null)
                {
                    yield break;
                }
                image.sprite = request.Asset;
            }
            yield return null;
            if (image != null)
            {
                image.color = Color.white;
            }
        }

        private void createStoreItems()
        {
            int count = franchiseDef.Items.Count;
            for (int i = 0; i < count; i++)
            {
                DisneyStoreItemDefinition definition = franchiseDef.Items[i];
                GameObject gameObject = UnityEngine.Object.Instantiate(itemPrefab, ItemContainer);
                gameObject.GetComponent<DisneyStoreFranchiseItem>().SetItem(new DisneyStoreItemData(definition), this);
            }
        }

        public void ShowConfirmation(DisneyStoreItemData item, Sprite icon, DisneyStoreFranchiseItem shopItem)
        {
            PrefabContentKey prefabKey = OwnedConfirmationPrefabKey;
            bool flag = DisneyStoreUtils.IsItemMultiPurchase(item);
            if (DisneyStoreUtils.IsIglooReward(item))
            {
                prefabKey = IglooConfirmationPrefabKey;
            }
            else if (flag)
            {
                prefabKey = ConsumableConfirmationPrefabKey;
            }
            else if (!DisneyStoreUtils.IsItemOwned(item))
            {
                prefabKey = PurchaseConfirmationPrefabKey;
            }
            CoroutineRunner.Start(loadConfirmation(prefabKey, item, icon, shopItem), this, "");
        }

        public void HideConfirmation()
        {
            if (confirmation != null)
            {
                UnityEngine.Object.Destroy(confirmation);
                ContentScrollRect.onValueChanged.RemoveListener(onContentScrollRectValueChanged);
            }
        }

        private bool isNewTouch()
        {
            return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        }

        private IEnumerator loadConfirmation(PrefabContentKey prefabKey, DisneyStoreItemData item, Sprite icon, DisneyStoreFranchiseItem shopItem)
        {
            AssetRequest<GameObject> request = Content.LoadAsync(prefabKey);
            yield return request;
            GameObject newConfirmation = UnityEngine.Object.Instantiate(request.Asset, ConfirmationContainer, false);
            newConfirmation.GetComponent<AbstractDisneyStoreConfirmation>().SetItem(item, icon, this, storeController, shopItem, ContentScrollRect.transform as RectTransform);
            if (confirmation != null)
            {
                HideConfirmation();
            }
            confirmation = newConfirmation;
            ContentScrollRect.onValueChanged.AddListener(onContentScrollRectValueChanged);
        }

        private void OnDestroy()
        {
            HideConfirmation();
            CoroutineRunner.StopAllForOwner(this);
        }

        private void onContentScrollRectValueChanged(Vector2 scrollDelta)
        {
            HideConfirmation();
        }
    }
}