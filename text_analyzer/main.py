import os

def count_lines(content):
    return len(content.splitlines())

def count_chars(content):
    return len(content)

def count_empty_lines(content):
    return sum(1 for line in content.splitlines() if line.strip() == '')

def char_frequency(content):
    freq = {}
    for char in content:
        freq[char] = freq.get(char, 0) + 1
    return dict(sorted(freq.items(), key=lambda x: x[1], reverse=True))

def print_menu():
    print("\nВыберите информацию для отображения:")
    print("1 - Количество строк")
    print("2 - Количество символов")
    print("3 - Количество пустых строк")
    print("4 - Частотный словарь символов")
    print("0 - Выход")

def main():
    filename = input("Введите имя файла: ")
    
    script_dir = os.path.dirname(os.path.abspath(__file__))
    filepath = os.path.join(script_dir, filename)
    
    try:
        with open(filepath, 'r', encoding='utf-8') as file:
            content = file.read()
    except FileNotFoundError:
        print(f"Ошибка: файл '{filename}' не найден в директории {script_dir}")
        return
    except Exception as e:
        print(f"Ошибка при чтении файла: {e}")
        return
    
    while True:
        print_menu()
        choice = input("\nВведите номера через пробел (например: 1 2 4) или 0 для выхода: ")
        
        if choice.strip() == '0':
            break
        
        choices = choice.split()
        print("\n" + "="*50)
        
        for c in choices:
            if c == '1':
                print(f"Количество строк: {count_lines(content)}")
            elif c == '2':
                print(f"Количество символов: {count_chars(content)}")
            elif c == '3':
                print(f"Количество пустых строк: {count_empty_lines(content)}")
            elif c == '4':
                print("Частотный словарь символов:")
                freq = char_frequency(content)
                for char, count in freq.items():
                    display_char = repr(char) if char in '\n\t\r' else char
                    print(f"  {display_char}: {count}")
            else:
                print(f"Неизвестный выбор: {c}")
        
        print("="*50)
main()