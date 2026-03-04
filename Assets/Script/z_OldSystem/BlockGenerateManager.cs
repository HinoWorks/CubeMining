using UnityEngine;
using System.Collections.Generic;
using UniRx;


namespace OldSystem
{

    /*
        [System.Serializable]
        public class GenerateBlockData
        {
            public BlockGenerateParam param { get; private set; }
            private float timer = 0f;
            public BlockSize sizeType => Random.Range(0f, 1f) < param.bigBlockRate ? BlockSize.Big : BlockSize.Normal;

            public void Init(BlockGenerateParam _param)
            {
                param = _param;
            }
            public void UnityUpdate()
            {
                timer += Time.deltaTime;
                if (timer >= param.generateInterval)
                {
                    BlockGenerateManager.Inst.GenerateBlock(this, sizeType);
                    timer = 0f;
                }
            }

        }


        [System.Serializable]
        public class GenerateObjectData
        {
            public ObjectGenerateParam param { get; private set; }
            private float checkInterval = 1f;
            private float timer = 0f;
            public void Init(ObjectGenerateParam _param)
            {
                param = _param;
            }
            public void UnityUpdate()
            {
                if (param.generateRate <= 0f) return;
                timer += Time.deltaTime;
                if (timer >= checkInterval)
                {
                    BlockGenerateManager.Inst.GenerateObject(this);
                    timer = 0f;
                }
            }

        }


        public class BlockGenerateManager : MonoBehaviour
        {
            public static BlockGenerateManager Inst;
            [SerializeField] Vector2 range_x;
            [SerializeField] Vector2 range_y;
            [SerializeField] Vector2 range_z;


            // -- loc
            private List<MiningTarget_Cube> list_targetBlocks = new List<MiningTarget_Cube>(); // 生成されたブロックのリスト
            private List<GenerateBlockData> list_generateBlockDatas = new List<GenerateBlockData>(); // 生成されるブロックのデータリスト

            private List<MiningTarget_Object> list_targetObjects = new List<MiningTarget_Object>(); // 生成されたオブジェクトのリスト
            private List<GenerateObjectData> list_objectGenerateDatas = new List<GenerateObjectData>(); // 生成されるオブジェクトのデータリスト

            public BlockGenerateParam blockGenerateParam_max { get; private set; } // 最大ブロックパラメータ
            private bool isGenerate = false;
            private int initialGenerateCount = 15;

            private float bigBlockSizeRate = 2f;
            private Vector3[] array_position = new Vector3[6]
            {
            new Vector3(1, 0, -1),new Vector3(-1, 1, 1),new Vector3(1, 0, 1),
            new Vector3(-1, 1, -1),new Vector3(-1, 0, 1),new Vector3(1, 1, 1),
            };



            void Awake()
            {
                if (Inst == null) { Inst = this; }
                else { Destroy(this); }
            }



            void Update()
            {
                if (!isGenerate) return;

                for (int i = 0; i < list_generateBlockDatas.Count; i++)
                {
                    list_generateBlockDatas[i].UnityUpdate();
                }
                for (int i = 0; i < list_objectGenerateDatas.Count; i++)
                {
                    list_objectGenerateDatas[i].UnityUpdate();
                }
            }

            public void Init()
            {
                Set_BlockGenerateDatas();
                Set_ObjectGenerateDatas();

                // pool init
                var targetBlockData = list_generateBlockDatas[0];
                for (int i = 0; i < initialGenerateCount; i++)
                {
                    GenerateBlock(targetBlockData, targetBlockData.sizeType);
                }

                var targetBlockData2 = list_generateBlockDatas[1];
                for (int i = 0; i < initialGenerateCount; i++)
                {
                    GenerateBlock(targetBlockData2, targetBlockData2.sizeType);
                }

            }
            public void Set_GenerateState(bool _state)
            {
                isGenerate = _state;
            }
            public void ResetAllBlocks()
            {
                foreach (var targetBlock in list_targetBlocks)
                {
                    targetBlock.NotActivate();
                }
                foreach (var targetObject in list_targetObjects)
                {
                    targetObject.NotActivate();
                }
            }




            #region == Block Generate ==
            private void Set_BlockGenerateDatas()
            {
                list_generateBlockDatas.Clear();
                foreach (var blockData in GameParamManager.list_blockGenerateParam)
                {
                    if (!blockData.isActive) continue;
                    var generateBlockData = new GenerateBlockData();
                    generateBlockData.Init(blockData);
                    list_generateBlockDatas.Add(generateBlockData);
                    blockGenerateParam_max = blockData;
                }
            }

            public void GenerateBlock(GenerateBlockData _blockData, BlockSize _blockSizeType)
            {
                for (int i = 0; i < _blockData.param.count; i++)
                {
                    var targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false && x.index == _blockData.param.blockIndex);
                    if (targetBlock == null)
                    {
                        var newBlock = Instantiate(_blockData.param.so.pf, InGameManager.Inst.ParentPool) as GameObject;
                        targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
                        list_targetBlocks.Add(targetBlock);
                    }

                    targetBlock.transform.position = GetRandomPosition();
                    targetBlock.transform.rotation = GetRandomRotation();

                    targetBlock.Init(_blockData.param.hp, _blockData.param.baseValue, _blockData.param.blockIndex, 0);
                    var blockSizeRate = _blockSizeType == BlockSize.Big ? bigBlockSizeRate : 1f;
                    targetBlock.Set_BlockSize(_blockSizeType, _blockData.param.size * blockSizeRate);
                }
            }
            private Vector3 GetRandomPosition()
            {
                return new Vector3(Random.Range(range_x.x, range_x.y), Random.Range(range_y.x, range_y.y), Random.Range(range_z.x, range_z.y));
            }
            private Quaternion GetRandomRotation()
            {
                return Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            }

            public void BreakBigBlock(int _blockIndex, Vector3 _position)
            {
                var blockData = GameParamManager.list_blockGenerateParam.Find(x => x.blockIndex == _blockIndex);
                if (blockData == null) return;
                for (int i = 0; i < blockData.separateBlockCount; i++)
                {
                    var targetBlock = list_targetBlocks.Find(x => x.isActiveAndEnabled == false && x.index == blockData.blockIndex);
                    if (targetBlock == null)
                    {
                        var newBlock = Instantiate(blockData.so.pf, InGameManager.Inst.ParentPool) as GameObject;
                        targetBlock = newBlock.GetComponent<MiningTarget_Cube>();
                        list_targetBlocks.Add(targetBlock);
                    }

                    targetBlock.transform.position = _position + 0.3f * array_position[i];
                    targetBlock.transform.rotation = Quaternion.identity;
                    targetBlock.Init(blockData.hp, blockData.baseValue, blockData.blockIndex, 0);
                    targetBlock.Set_BlockSize(BlockSize.Normal, blockData.size);
                }
            }
            #endregion



            #region == Object Generate ==
            private void Set_ObjectGenerateDatas()
            {
                list_objectGenerateDatas.Clear();
                foreach (var objectData in GameParamManager.list_objectGenerateParam)
                {
                    var generateObjectData = new GenerateObjectData();
                    generateObjectData.Init(objectData);
                    list_objectGenerateDatas.Add(generateObjectData);
                }
            }
            public void GenerateObject(GenerateObjectData _objectData)
            {
                var targetObject = list_targetObjects.Find(x => x.isActiveAndEnabled == false
                    && x.index == _objectData.param.so.objectIndex);
                if (targetObject == null)
                {
                    var newObject = Instantiate(_objectData.param.so.pf, InGameManager.Inst.ParentPool) as GameObject;
                    targetObject = newObject.GetComponent<MiningTarget_Object>();
                    list_targetObjects.Add(targetObject);
                }
                targetObject.transform.position = GetRandomPosition();
                targetObject.transform.rotation = GetRandomRotation();
                targetObject.Init(_objectData.param, null, 0);
            }
            #endregion



            public MiningTargetBase Get_RandomTargetBlock()
            {
                var activeBlocks = list_targetBlocks.FindAll(x => x.isActiveAndEnabled);
                if (activeBlocks.Count == 0) return null;
                return activeBlocks[Random.Range(0, activeBlocks.Count)];
            }

        }
        */
}
