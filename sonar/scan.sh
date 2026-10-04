#!/usr/bin/env bash
#
# Сканирование .NET-решения в SonarQube.
# Токен и хост берутся из .env.sonar, потом очищаются.
#
# Использование:
#   ./sonar/scan.sh MyCompany.sln

set -euo pipefail

# Аргументы
if [[ $# -lt 1 ]]; then
  echo "Использование: $0 <path/to/solution.sln>"
  exit 1
fi

SLN="$1"

if [[ ! -f "$SLN" ]]; then
  echo "Файл не найден: $SLN"
  exit 1
fi

# Загрузка .env.sonar
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
ENV_FILE="${ENV_FILE:-$ROOT_DIR/.env.sonar}"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Не найден файл окружения: $ENV_FILE"
  exit 1
fi

# set -a → переменные из файла автоматически экспортируются
set -a
source "$ENV_FILE"
set +a

# Проверка обязательных переменных
: "${SONAR_TOKEN:?SONAR_TOKEN не задан в $ENV_FILE}"
SONAR_HOST_URL="${SONAR_HOST_URL:-http://localhost:9000}"

cleanup() {
  unset SONAR_TOKEN
  unset SONAR_DB_PASSWORD
}
trap cleanup EXIT

# Подготовка
SLN_DIR="$(cd "$(dirname "$SLN")" && pwd)"
SLN_NAME="$(basename "$SLN")"

# Git Bash / MSYS2 на Windows
case "${OSTYPE:-}" in
  msys*|cygwin*) export MSYS_NO_PATHCONV=1 ;;
esac


# Скан
cd "$SLN_DIR"

echo ">  solution:   $SLN_NAME"
echo ">  directory:  $SLN_DIR"
echo ">  projectKey: $PROJECT_KEY"
echo ">  sonar:      $SONAR_HOST_URL"
echo ">  env file:   $ENV_FILE"

echo "[1/3] sonarscanner begin"
dotnet sonarscanner begin \
  "/k:${PROJECT_KEY}" \
  "/d:sonar.host.url=${SONAR_HOST_URL}" \
  "/d:sonar.login=${SONAR_TOKEN}"

echo
echo "[2/3] dotnet build --no-incremental -c Release"
dotnet build "$SLN_NAME" --no-incremental -c Release

echo
echo "[3/3] sonarscanner end"
dotnet sonarscanner end \
  "/d:sonar.login=${SONAR_TOKEN}"

echo
echo "Готово: $SONAR_HOST_URL/dashboard?id=$PROJECT_KEY"