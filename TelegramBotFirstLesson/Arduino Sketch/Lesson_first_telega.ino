// Определяем пин, к которому подключен зуммер
int buzzerPin = A0;

void setup() {
  // Настраиваем серийное соединение со скоростью 9600 бод
  Serial.begin(9600);
  // Настраиваем пин зуммера как выход
  pinMode(buzzerPin, OUTPUT);
}

void loop() {
  // Проверяем, доступны ли данные для чтения в серийном порту
  if (Serial.available() > 0) {
    // Читаем строку из серийного порта
    String incomingString = Serial.readString();
    
    // Убираем все лишние пробелы и переносы строки для чистого сравнения
    incomingString.trim();
    
    // Сравниваем полученную строку со строкой "beep"
    if (incomingString == "beep") {
      // Активируем зуммер на частоте 500 Гц на одну секунду
      tone(buzzerPin, 500, 1000);
      // Отправляем уведомление обратно в серийный порт
      Serial.println("Buzzer on pin " + String(buzzerPin) + " was successfully activated.");
    }
    // Если строка не совпадает, ничего не делаем
  }
}
