# marti-tech-demo

.Net Core 2.2
MSUnit
Swagger

/swagger adresinden API metodları test edilebilir.

Unit testler visual studio içindeki test explorer ile çalıştırılabilir.

Basit bir expire-token yetkilendirmesi eklendi.

--- Eğer alınan token swagger üzerinden token olarka eklenmez ise 401 hatası alınacaktır.

Api üzerindeki 3 metoddan biri token yaratmak, diğeri veriyi upload etmek, sonuncusu da bu veriyi filtrelemek ve aramak için kullanıldı.
Token mekanizması için middleware mantığı kullanıldı.
Yaklaşım olarak en az kod ile en çok işlevsellik hedeflendi.
Gereksinimlerde olmadığı için persistent bir veri tutma mekanizması kullanılmadı.
Dependency injection mekanizmasından faydalanıldı.
Çok fazla veri olduğunda nesting ile sorun yaşanacağından arama algoritması flat csv listesine dönüştürülüp işlendi.

Modeller için unit test yazılmadı, coverage için yüzde 40 altı hedeflendi.
Assembly içerisinde gelen metodlar unit testing coverage'dan exclude edilmedi.

Solution'u build etmek veya başka bir konuyla ilgili sorun yaşanır ise tughanbelbek [at] gmail.com 'dan bana ulaşabilirsiniz.