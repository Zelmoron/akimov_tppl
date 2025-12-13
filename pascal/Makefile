.PHONY: all build run test coverage clean help


all: build


build:
	@echo "Сборка проекта..."
	dotnet build PascalInterpreter/PascalInterpreter.csproj

run: build
	@echo "Запуск интерпретатора Pascal..."
	dotnet run --project PascalInterpreter/PascalInterpreter.csproj


test: build
	@echo "Запуск тестов..."
	dotnet test PascalInterpreter.Tests/PascalInterpreter.Tests.csproj


coverage: build
	@echo "Запуск тестов с покрытием кода..."
	dotnet test PascalInterpreter.Tests/PascalInterpreter.Tests.csproj \
		--collect:"XPlat Code Coverage" \
		--results-directory ./TestResults
	@echo ""
	@echo "Генерация HTML отчёта..."
	dotnet tool restore 2>/dev/null || true
	dotnet tool run reportgenerator \
		"-reports:./TestResults/*/coverage.cobertura.xml" \
		"-targetdir:./TestResults/html" \
		"-reporttypes:Html"
	@echo ""
	@echo "Отчёт о покрытии сохранён в ./TestResults/html/index.html"


clean:
	@echo "Очистка проекта..."
	dotnet clean PascalInterpreter/PascalInterpreter.csproj
	dotnet clean PascalInterpreter.Tests/PascalInterpreter.Tests.csproj
	rm -rf PascalInterpreter/bin PascalInterpreter/obj
	rm -rf PascalInterpreter.Tests/bin PascalInterpreter.Tests/obj


help:
	@echo "Доступные команды:"
	@echo "  make build    - Собрать проект"
	@echo "  make run      - Запустить интерпретатор"
	@echo "  make test     - Запустить тесты"
	@echo "  make coverage - Запустить тесты с покрытием кода"
	@echo "  make clean    - Очистить проект"
	@echo "  make help     - Показать эту справку"
