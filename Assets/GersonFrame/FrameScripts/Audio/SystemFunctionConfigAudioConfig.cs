// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: SystemFunctionConfigAudioConfig.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;
namespace HotDragonRun.Proto {

  #region Messages
  public sealed class SystemFunctionConfigAudioConfigConfig : pb::IMessage {
    private static readonly pb::MessageParser<SystemFunctionConfigAudioConfigConfig> _parser = new pb::MessageParser<SystemFunctionConfigAudioConfigConfig>(() => new SystemFunctionConfigAudioConfigConfig());
    public static pb::MessageParser<SystemFunctionConfigAudioConfigConfig> Parser { get { return _parser; } }

    private int iD_;
    /// <summary>
    ///音效id
    /// </summary>
    public int ID {
      get { return iD_; }
      set {
        iD_ = value;
      }
    }

    private string audioName_ = "";
    /// <summary>
    ///音效名字
    /// </summary>
    public string AudioName {
      get { return audioName_; }
      set {
        audioName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    private string audioType_ = "";
    /// <summary>
    ///音效类型
    /// </summary>
    public string AudioType {
      get { return audioType_; }
      set {
        audioType_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    private string audioPosition_ = "";
    /// <summary>
    ///音效位置
    /// </summary>
    public string AudioPosition {
      get { return audioPosition_; }
      set {
        audioPosition_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    private string desc_ = "";
    /// <summary>
    ///音效描述
    /// </summary>
    public string Desc {
      get { return desc_; }
      set {
        desc_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    private float volume_;
    /// <summary>
    ///音量
    /// </summary>
    public float Volume {
      get { return volume_; }
      set {
        volume_ = value;
      }
    }

    private bool loop_;
    /// <summary>
    ///是否循环播放
    /// </summary>
    public bool Loop {
      get { return loop_; }
      set {
        loop_ = value;
      }
    }

    private float delay_;
    /// <summary>
    ///延迟播放时间
    /// </summary>
    public float Delay {
      get { return delay_; }
      set {
        delay_ = value;
      }
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (ID != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(ID);
      }
      if (AudioName.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(AudioName);
      }
      if (AudioType.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(AudioType);
      }
      if (AudioPosition.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(AudioPosition);
      }
      if (Desc.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(Desc);
      }
      if (Volume != 0F) {
        output.WriteRawTag(53);
        output.WriteFloat(Volume);
      }
      if (Loop != false) {
        output.WriteRawTag(56);
        output.WriteBool(Loop);
      }
      if (Delay != 0F) {
        output.WriteRawTag(69);
        output.WriteFloat(Delay);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (ID != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ID);
      }
      if (AudioName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(AudioName);
      }
      if (AudioType.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(AudioType);
      }
      if (AudioPosition.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(AudioPosition);
      }
      if (Desc.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Desc);
      }
      if (Volume != 0F) {
        size += 1 + 4;
      }
      if (Loop != false) {
        size += 1 + 1;
      }
      if (Delay != 0F) {
        size += 1 + 4;
      }
      return size;
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            ID = input.ReadInt32();
            break;
          }
          case 18: {
            AudioName = input.ReadString();
            break;
          }
          case 26: {
            AudioType = input.ReadString();
            break;
          }
          case 34: {
            AudioPosition = input.ReadString();
            break;
          }
          case 42: {
            Desc = input.ReadString();
            break;
          }
          case 53: {
            Volume = input.ReadFloat();
            break;
          }
          case 56: {
            Loop = input.ReadBool();
            break;
          }
          case 69: {
            Delay = input.ReadFloat();
            break;
          }
        }
      }
    }

  }

  public sealed class SystemFunctionConfigAudioConfigConfigData : pb::IMessage {
    private static readonly pb::MessageParser<SystemFunctionConfigAudioConfigConfigData> _parser = new pb::MessageParser<SystemFunctionConfigAudioConfigConfigData>(() => new SystemFunctionConfigAudioConfigConfigData());
    public static pb::MessageParser<SystemFunctionConfigAudioConfigConfigData> Parser { get { return _parser; } }

    private static readonly pb::FieldCodec<global::HotDragonRun.Proto.SystemFunctionConfigAudioConfigConfig> _repeated_systemFunctionConfigAudioConfigConfigs_codec
        = pb::FieldCodec.ForMessage(10, global::HotDragonRun.Proto.SystemFunctionConfigAudioConfigConfig.Parser);
    private readonly pbc::RepeatedField<global::HotDragonRun.Proto.SystemFunctionConfigAudioConfigConfig> systemFunctionConfigAudioConfigConfigs_ = new pbc::RepeatedField<global::HotDragonRun.Proto.SystemFunctionConfigAudioConfigConfig>();
    public pbc::RepeatedField<global::HotDragonRun.Proto.SystemFunctionConfigAudioConfigConfig> SystemFunctionConfigAudioConfigConfigs {
      get { return systemFunctionConfigAudioConfigConfigs_; }
    }

    public void WriteTo(pb::CodedOutputStream output) {
      systemFunctionConfigAudioConfigConfigs_.WriteTo(output, _repeated_systemFunctionConfigAudioConfigConfigs_codec);
    }

    public int CalculateSize() {
      int size = 0;
      size += systemFunctionConfigAudioConfigConfigs_.CalculateSize(_repeated_systemFunctionConfigAudioConfigConfigs_codec);
      return size;
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            systemFunctionConfigAudioConfigConfigs_.AddEntriesFrom(input, _repeated_systemFunctionConfigAudioConfigConfigs_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
