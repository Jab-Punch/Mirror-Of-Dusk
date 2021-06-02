using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectCharacterShard : AbstractCollidableObject
{
    private CharacterSelectPlayer player;
    public PlayerId id { get; private set; }
    [NonSerialized] public int characterSelectedId = -1;
    [NonSerialized] public int characterColorCode = 0;
    [NonSerialized] public State state;

    [Header("Components")]
    [SerializeField] CharacterSelectCharacterShardAnimator shardAnimator;

    [Header("Sprites")]
    [SerializeField] SpriteRenderer sprRenderCharacterIcon0;
    [SerializeField] SpriteRenderer sprRenderCharacterIcon1;
    [SerializeField] SpriteRenderer sprRenderBackground0;
    [SerializeField] SpriteRenderer sprRenderBackground1;
    [SerializeField] CharacterSprite[] characterSprites;

    private bool _initialized = false;
    private bool hitboxActive = false;
    private Vector3 restPosition;
    private Vector3 activePosition;
    [NonSerialized] public SummonState summonState;
    private IEnumerator _moveShardUp;
    private IEnumerator _moveShardDown;

    public IEnumerator moveShardUp
    {
        get
        {
            this._moveShardUp = moveBase_cr(this.restPosition, this.activePosition, true);
            return _moveShardUp;
        }
    }

    public IEnumerator moveShardDown
    {
        get
        {
            this._moveShardDown = moveBase_cr(this.activePosition, this.restPosition, false);
            return _moveShardDown;
        }
    }

    [Serializable]
    public class CharacterSprite
    {
        public Sprite normal;
        public Sprite reflection;
    }

    public enum SummonState
    {
        Opening,
        Instant
    }

    public enum State
    {
        Off,
        On
    }

    [Serializable]
    public class InitObject
    {
        public Vector2 position;

        public InitObject(Vector2 position)
        {
            this.position = position;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_initialized)
        {
            this.state = State.Off;
            /*this.sprRenderCharacterIcon0.sprite = this.characterSprites[0].normal;
            this.sprRenderCharacterIcon1.sprite = this.characterSprites[0].reflection;*/
            characterSelectedId = -1;
            characterColorCode = 0;
            CharacterIconResources.CharacterIconData data = CharacterSelectScene.Current.characterIconData.data[0];
            this.shardAnimator.IconRenderer.sprite = data.normal._sprite;
            this.shardAnimator.IconRenderer2.sprite = data.reflection._sprite;
            this.shardAnimator.IconRenderer.sharedMaterial = data.normal.selectPalettes[characterColorCode].bustUpPalette;
            this.shardAnimator.IconRenderer2.sharedMaterial = data.reflection.selectPalettes[characterColorCode].bustUpPalette;
            this.shardAnimator.BackgroundRenderer0.color = data.backgroundColor0;
            this.shardAnimator.BackgroundRenderer1.color = data.backgroundColor1;
            if (summonState == SummonState.Opening)
            {
                this.StartCoroutine(this.moveShardUp);
                this.shardAnimator.Play("Stationary");
            }
            else
            {
                this.gameObject.transform.position = this.activePosition;
                this.shardAnimator.Play("Stationary");
                this.hitboxActive = true;
                this.state = State.On;
            }
        }
    }

    protected void OnDisable()
    {
        this.hitboxActive = false;
    }

    protected override void Update()
    {
        base.Update();
    }

    public static CharacterSelectCharacterShard Create(CharacterSelectPlayer player, CharacterSelectCharacterShard.InitObject init)
    {
        CharacterSelectCharacterShard characterSelectCharacterShard = UnityEngine.Object.Instantiate<CharacterSelectCharacterShard>
            (CharacterSelectScene.Current.characterSelectCharacterShard);
        characterSelectCharacterShard.Init(player, init);
        return characterSelectCharacterShard;
    }

    private void Init(CharacterSelectPlayer player, CharacterSelectCharacterShard.InitObject init)
    {
        this.player = player;
        base.gameObject.name = "PlayerCharacterShard_" + player.playerId.ToString();
        this.id = player.playerId;
        this.restPosition = init.position;
        this.activePosition = new Vector3(this.restPosition.x, this.restPosition.y + 580f, 200f);
        base.transform.position = this.restPosition;
        this.sprRenderCharacterIcon0.sprite = this.characterSprites[0].normal;
        this.sprRenderCharacterIcon1.sprite = this.characterSprites[0].reflection;
        _initialized = true;
        this.gameObject.SetActive(false);
    }

    private IEnumerator moveBase_cr(Vector3 startPos, Vector3 endPos, bool summon)
    {
        yield return base.TweenPositionVec3(startPos, endPos, 20f, EaseUtils.EaseType.linear);
        if (summon)
        {
            this.hitboxActive = true;
            this.state = State.On;
        } else
        {
            this.hitboxActive = false;
            this.state = State.Off;
            this.gameObject.SetActive(false);
        }
        yield break;
    }

    public void UpdateIcons(int foundCharacterId, int foundColorCode)
    {
        if (this.state == State.On)
        {
            characterSelectedId = foundCharacterId;
            characterColorCode = foundColorCode;
            CharacterSelectScene.Current.playerData[(int)this.id].characterColorCode = this.characterColorCode;
            int characterId = GetCharacterId(foundCharacterId);
            CharacterIconResources.CharacterIconData data = CharacterSelectScene.Current.characterIconData.data[characterId];
            this.shardAnimator.IconRenderer.sprite = data.normal._sprite;
            this.shardAnimator.IconRenderer2.sprite = data.reflection._sprite;
            this.shardAnimator.IconRenderer.sharedMaterial = data.normal.selectPalettes[characterColorCode].bustUpPalette;
            this.shardAnimator.IconRenderer2.sharedMaterial = data.reflection.selectPalettes[characterColorCode].bustUpPalette;
            this.shardAnimator.BackgroundRenderer0.color = data.backgroundColor0;
            this.shardAnimator.BackgroundRenderer1.color = data.backgroundColor1;
        }
    }

    public void UpdateCharacterColor(int code)
    {
        if (this.state == State.On)
        {
            int characterId = GetCharacterId(characterSelectedId);
            CharacterIconResources.CharacterIconData data = CharacterSelectScene.Current.characterIconData.data[characterId];
            this.characterColorCode = ((code + data.normal.selectPalettes.Length)) % data.normal.selectPalettes.Length;
            CharacterSelectScene.Current.playerData[(int)this.id].characterColorCode = this.characterColorCode;
            UpdateIcons(characterId, this.characterColorCode);
        }
    }

    public void ShiftCharacterColor(int amount)
    {
        if (this.state == State.On)
        {
            int characterId = GetCharacterId(characterSelectedId);
            CharacterIconResources.CharacterIconData data = CharacterSelectScene.Current.characterIconData.data[characterId];
            this.characterColorCode = ((this.characterColorCode + data.normal.selectPalettes.Length) + amount) % data.normal.selectPalettes.Length;
            CharacterSelectScene.Current.playerData[(int)this.id].characterColorCode = this.characterColorCode;
            UpdateIcons(characterId, this.characterColorCode);
        }
    }

    public int GetCharacterId(int cId)
    {
        int characterId = cId;
        if (characterId >= CharacterSelectScene.Current.characterIconData.data.Length || characterId == 0 || characterId == -1)
        {
            characterId = 0;
        }
        return characterId;
    }

    public void DisableThisShard()
    {
        this.summonState = SummonState.Opening;
        this.StopThisCoroutine(_moveShardUp);
        this.StopThisCoroutine(_moveShardDown);
        this.StartCoroutine(moveShardDown);
    }

    private void StopThisCoroutine(IEnumerator cr)
    {
        if (cr != null)
            this.StopCoroutine(cr);
    }

    public void Play(string animation)
    {
        this.shardAnimator.Play(animation);
    }
}
