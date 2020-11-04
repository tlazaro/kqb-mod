using System;
using GameLogic;
using LiquidBit.KillerQueenX.Extensions;
using UnityEngine;

// Token: 0x0200006C RID: 108
namespace KQBMod.Training
{

    public class AttackVolumeVisual : MonoBehaviour
    {
        // Token: 0x060002ED RID: 749 RVA: 0x000120CC File Offset: 0x000102CC
        private void Start()
        {
            this.entityState = base.GetComponent<EntityState>();
            this.spriteRenderer = base.GetComponent<SpriteRenderer>();
            this.spriteRenderer.enabled = false;
            this.ownerSortOrder = -1;
            this.boxSprite = GameManager.GMInstance.assetSystem.LoadAsset<Sprite>("DebugTexture");
            this.circleSprite = GameManager.GMInstance.assetSystem.LoadAsset<Sprite>("UnitCircle");
        }

        // Token: 0x060002EE RID: 750 RVA: 0x00012138 File Offset: 0x00010338
        private void Update()
        {
            Entity entityStateForVisuals = this.entityState.entityStateForVisuals;
            bool alwaysShowHitBoxes = Game.gameConfiguration.debugParams.alwaysShowHitBoxes;
            if (entityStateForVisuals == null || !alwaysShowHitBoxes)
            {
                this.spriteRenderer.enabled = false;
                return;
            }
            Entity parent = this.entityState.game.GetParent(entityStateForVisuals);
            if (parent != null)
            {
                if (this.ownerSortOrder == -1)
                {
                    this.ownerSortOrder = MatchManager.Instance.gameObjects[parent.id].gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder;
                    this.spriteRenderer.sortingOrder = this.ownerSortOrder + 1;
                }
                if (entityStateForVisuals.colliderVolume.type == Entity.ColliderVolumeType.Box)
                {
                    this.spriteRenderer.transform.localScale = entityStateForVisuals.dim.ToUnityV2();
                    this.spriteRenderer.sprite = this.boxSprite;
                }
                else
                {
                    this.spriteRenderer.transform.localScale = new Vector3(entityStateForVisuals.radius, entityStateForVisuals.radius, 1f);
                    this.spriteRenderer.sprite = this.circleSprite;
                }
                this.spriteRenderer.transform.position = entityStateForVisuals.pos.ToUnityV2();
                if (parent.teamColor == Team.Color.Red)
                {
                    this.spriteRenderer.color = this.redColor;
                }
                else
                {
                    this.spriteRenderer.color = this.blueColor;
                }
                this.spriteRenderer.enabled = entityStateForVisuals.collides;
                return;
            }
            this.spriteRenderer.enabled = false;
        }

        // Token: 0x04000280 RID: 640
        private SpriteRenderer spriteRenderer;

        // Token: 0x04000281 RID: 641
        private EntityState entityState;

        // Token: 0x04000282 RID: 642
        private Color redColor = new Color(0.93333334f, 0.24705882f, 0.24705882f, 0.8f);

        // Token: 0x04000283 RID: 643
        private Color blueColor = new Color(0.93333334f, 0.24705882f, 0.24705882f, 0.8f);

        // Token: 0x04000284 RID: 644
        private int ownerSortOrder;

        // Token: 0x04000285 RID: 645
        private Sprite boxSprite;

        // Token: 0x04000286 RID: 646
        private Sprite circleSprite;
    }



}