import pytest

from plib import Point

@pytest.fixture
def points():
    return Point(0, 0), Point(2, 2)

class TestPoint:

    def test_creation(self):
        p = Point(1, 2)
        assert p.x == 1 and p.y == 2

        with pytest.raises(TypeError):
            Point(1.5, 1.5)

    def test_add(self, points):
        p1, p2 = points
        assert p2 + p1 == Point(2, 2)
    
    def test_iadd(self, points):
        p1, p2 = points
        p1 += p2
        assert p1 == Point(2, 2)
    
    def test_sub(self, points):
        p1, p2 = points
        assert p2 - p1 == Point(2, 2)
        assert p1 - p2 == -Point(2, 2)
    
    def test_eq_with_non_point(self):
        p = Point(1, 2)
        with pytest.raises(NotImplementedError):
            p == "not a point"
    
    def test_distance_to(self):
        p1 = Point(0, 0)
        p2 = Point(2, 0)
        assert p1.to(p2) == 2

    @pytest.mark.parametrize(
            "p1, p2, distance",
            [(Point(0, 0), Point(0, 10), 10),
             (Point(0, 0), Point(10, 0), 10),
             (Point(0, 0), Point(1, 1), 1.414)]
    )
    def test_distance_all_axis(self, p1, p2, distance):
        assert p1.to(p2) == pytest.approx(distance, 0.001)
    
    def test_str(self):
        p = Point(3, 4)
        assert str(p) == "Point(3, 4)"
    
    def test_repr(self):
        p = Point(3, 4)
        assert repr(p) == "Point(3, 4)"
    
    def test_is_center(self):
        p1 = Point(0, 0)
        p2 = Point(1, 1)
        assert p1.is_center() == True
        assert p2.is_center() == False
    
    def test_to_json(self):
        p = Point(3, 4)
        json_str = p.to_json()
        assert json_str == '{"x": 3, "y": 4}'
    
    def test_from_json(self):
        json_str = '{"x": 3, "y": 4}'
        p = Point.from_json(json_str)
        assert p.x == 3 and p.y == 4
        assert p == Point(3, 4)