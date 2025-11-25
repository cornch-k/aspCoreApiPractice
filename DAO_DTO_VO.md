# DAO, DTO, VO

## DAO

DAO는 `Data Access Object`로, 데이터베이스의 데이터에 접근하기 위해 사용하는 객체. <br>
DAO의 목적은 **데이터베이스 접근 로직을 캡슐화** <br>
DAO를 사용하지 않는다면, DBMS를 변경할 때 모든 코드를 수정해야 함. 그러나 DAO를 사용하면 DAO 부분만 수정하면 됨.

## DTO

DTO는 `Data Transfer Object`로, 계층 간 데이터를 전달하기 위해 만든 객체.<br>
**순수하게 데이터만 담음.**

```
public struct CreateProductRequestDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
}
```

해당 DTO는 상품 생성에 필요한 정보만 담고 있고, DB에 추가 필드가 있더라도 DTO에는 포함시키지 않음.<br>
➡️ API의 계약이 명확해짐.

## VO

DTO는 `Data Transfer Object`로, 계층 간 데이터를 전달하기 위해 만든 객체.<br>
**Read Only 특징이 있음.**<br>
**DTO와 유사하지만, DTO는 Read/Write가 모두 가능함**
