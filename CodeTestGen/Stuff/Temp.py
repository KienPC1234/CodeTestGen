def _print_animated_banner() -> None:
    banner: List[str] = [
        r"╔════════════════════════════════════════════════════════════════════════════════╗",
        r"║ █████   ████   █████████  ██████████      ██████████   ██████████ █████   █████║",
        r"║░░███   ███░   ███░░░░░███░░███░░░░███    ░░███░░░░███ ░░███░░░░░█░░███   ░░███ ║",
        r"║ ░███  ███    ███     ░░░  ░███   ░░███    ░███   ░░███ ░███  █ ░  ░███    ░███ ║",
        r"║ ░███████    ░███          ░███    ░███    ░███    ░███ ░██████    ░███    ░███ ║",
        r"║ ░███░░███   ░███          ░███    ░███    ░███    ░███ ░███░░█    ░░███   ███  ║",
        r"║ ░███ ░░███  ░░███     ███ ░███    ███     ░███    ███  ░███ ░   █  ░░░█████░   ║",
        r"║ █████ ░░████ ░░█████████  ██████████      ██████████   ██████████    ░░███     ║",
        r"║░░░░░   ░░░░   ░░░░░░░░░  ░░░░░░░░░░      ░░░░░░░░░░   ░░░░░░░░░░      ░░░      ║",
        r"╚═════════════════════════════════════════════════ v1.0 ═════════════════════════╝"
    ]

    color: str = '\033[38;5;46m' 
    reset: str = '\033[0m'

    print("\033c", end="")  # Clear screen

    for line in banner:
        print(color + line + reset)
        time.sleep(0.05)  # Delay giữa các dòng (tùy chỉnh được)

    # Hiệu ứng nhấp nháy dòng cuối
    for _ in range(2):
        sys.stdout.write("\033[1A")  # Move cursor up
        sys.stdout.write(color + banner[-1] + reset + "\n")
        sys.stdout.flush()
        time.sleep(0.1)
        sys.stdout.write("\033[1A")
        sys.stdout.write("\033[38;5;82m" + banner[-1] + reset + "\n")
        sys.stdout.flush()
        time.sleep(0.1)

_print_animated_banner()