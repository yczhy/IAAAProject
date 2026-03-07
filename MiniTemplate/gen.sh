#!/bin/bash

# ================= 配置区域 =================
# 1. 必须填写你本机 dotnet 的绝对路径！
# 在终端运行 'which dotnet' 获取该路径
DOTNET_CMD="/usr/local/share/dotnet/dotnet" 

# 2. 验证 dotnet 是否存在，不存在则尝试自动查找
if [ ! -x "$DOTNET_CMD" ]; then
    # 尝试从常见 Homebrew 路径查找 (M1/M2/M3 Mac)
    if [ -x "/opt/homebrew/bin/dotnet" ]; then
        DOTNET_CMD="/opt/homebrew/bin/dotnet"
    elif [ -x "$HOME/.dotnet/dotnet" ]; then
        DOTNET_CMD="$HOME/.dotnet/dotnet"
    else
        # 最后尝试 source zshrc (在非交互 shell 中不一定生效，仅作兜底)
        if [ -f "$HOME/.zshrc" ]; then
            source "$HOME/.zshrc"
        fi
        DOTNET_CMD=$(which dotnet)
    fi
fi

# 最终检查
if [ ! -x "$DOTNET_CMD" ]; then
    echo "❌ 致命错误: 找不到 dotnet 可执行文件。"
    echo "请在 gen.sh 第 6 行手动指定正确的绝对路径。"
    echo "当前 PATH: $PATH"
    exit 1
fi

echo "✅ 使用 Dotnet: $DOTNET_CMD"
# ===========================================

# 原有逻辑
WORKSPACE=..
LUBAN_DLL=./Luban/Luban.dll
CONF_ROOT=.

echo "🚀 开始生成 Luban 数据..."
echo "工作目录: $(pwd)"

# 【关键】使用 $DOTNET_CMD 变量代替直接的 dotnet 命令
"$DOTNET_CMD" "$LUBAN_DLL" \
    -t all \
    -d json \
    -c cs-simple-json \
    --conf $CONF_ROOT/luban.conf \
    -x outputDataDir=$WORKSPACE/GameProject/Assets/GameFrame/LuBan/Datas \
    -x outputCodeDir=$WORKSPACE/GameProject/Assets/GameFrame/LuBan/Scripts

if [ $? -eq 0 ]; then
    echo "✅ 生成成功！"
else
    echo "❌ 生成失败！"
    exit 1
fi