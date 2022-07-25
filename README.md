# MemoryBackService
Проект процесса для автоматической публикации тезисов в Telegram-канал.

## How to start process?

In ```/etc/systemd/system``` create file ```*.service``` under ROOT priveleges.

Then paste to new file this:

```
[Unit]
Description=Memory Back Service

[Service]
Type=notify
ExecStart=<path_to_executable> --path<path_to_md_archive> --api<telegram_api_key>

[Install]
WantedBy=multi-user.target
```
