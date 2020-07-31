inline constexpr struct nil_t {} nil;

// Requirements: T is default-constructible and trivially-destructible
template<typename T>
struct nillable {
    bool has_value = false;
    T value;

    nillable() = default;

    nillable(nil_t) : nillable() {}
    nillable(const T& v) : has_value(true), value(v) {}

    nillable& operator=(nil_t) {
        has_value = false;
        return *this;
    }
    nillable& operator=(const T& v) {
        value = v;
        has_value = true;
        return *this;
    }

    const T* operator->() const { return &value; }
    T* operator->() { return &value; }

    explicit operator bool() const { return has_value; }
};

template<typename T> nillable(T)->nillable<T>;
