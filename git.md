
## git submodule操作

`git submodule add https://git.feinian.net/github_sync/LettuceEncrypt.git backend/LettuceEncrypt`

## 删除submodule

`git rm -f backend/LettuceEncrypt`

## 拉取submodule

### 第一次

`git submodule update --init --recursive`

### 更新

`git submodule update --recursive --remote`
