#pragma once

using size_t = decltype(alignof(char));

namespace type_traits {

template< class T > struct remove_cv { typedef T type; };
template< class T > struct remove_cv<const T> { typedef T type; };
template< class T > struct remove_cv<volatile T> { typedef T type; };
template< class T > struct remove_cv<const volatile T> { typedef T type; };
template< class T > using remove_cv_t = typename remove_cv<T>::type;

template< class T > struct remove_reference { typedef T type; };
template< class T > struct remove_reference<T&> { typedef T type; };
template< class T > struct remove_reference<T&&> { typedef T type; };
template< class T > using remove_reference_t = typename remove_reference<T>::type;

template< class T >
struct remove_cvref {
    using type = remove_cv_t<remove_reference_t<T>>;
};
template< class T > using remove_cvref_t = typename remove_cvref<T>::type;

namespace detail {

template <class T> struct type_identity { using type = T; };

template <class T>
auto try_add_lvalue_reference(int)->type_identity<T&>;
template <class T>
auto try_add_lvalue_reference(...)->type_identity<T>;

template <class T>
auto try_add_rvalue_reference(int)->type_identity<T&&>;
template <class T>
auto try_add_rvalue_reference(...)->type_identity<T>;

} // type_traits::detail

template <class T> struct add_lvalue_reference : decltype(detail::try_add_lvalue_reference<T>(0)) {};
template< class T > using add_lvalue_reference_t = typename add_lvalue_reference<T>::type;

template <class T> struct add_rvalue_reference : decltype(detail::try_add_rvalue_reference<T>(0)) {};
template< class T > using add_rvalue_reference_t = typename add_rvalue_reference<T>::type;

template <typename T, typename U> inline constexpr bool is_same_v = false;
template <typename T> inline constexpr bool is_same_v<T, T> = true;
template <typename T> inline constexpr bool is_void_v = is_same_v<remove_cv_t<T>, void>;

} // type_traits

template<class T> type_traits::add_rvalue_reference_t<T> declval() noexcept;

template <typename T>
inline constexpr T maxv(const T& a, const T& b) {
    return (b < a) ? a : b;
}

template <typename T>
inline constexpr T minv(const T& a, const T& b) {
    return (a < b) ? a : b;
}

template <typename T>
inline constexpr T clampv(const T& v, const T& lo, const T& hi) {
    return minv(maxv(v, lo), hi);
}

template <typename T>
inline constexpr const T& select_ref(bool pred, const T& t, const T& f) {
    return pred ? t : f;
}

template <typename First, typename... Rest>
inline constexpr size_t max_size_of = maxv(sizeof(First), max_size_of<Rest...>);

template <typename T>
inline constexpr size_t max_size_of<T> = sizeof(T);

template <typename First, typename... Rest>
inline constexpr size_t max_align_of = maxv(alignof(First), max_align_of<Rest...>);

template <typename T>
inline constexpr size_t max_align_of<T> = alignof(T);

namespace detail {

template <typename... Ts>
struct b1_index_of_impl {

    template <typename...>
    struct idx {
        static constexpr size_t value = 0;
    };

    template <typename T, typename First, typename... Rest>
    struct idx <T, First, Rest...> {
        static constexpr size_t value = []() {
            if constexpr (type_traits::is_same_v<T, First>) {
                return sizeof...(Ts) - sizeof...(Rest);
            }
            return idx<T, Rest...>::value;
        }();
    };
};

} // detail

template <typename T, typename First, typename... Rest>
inline constexpr int base1_index_of =
detail::b1_index_of_impl<First, Rest...>::template idx<T, First, Rest...>::value;

/*
Requirements: Every type is trivially-copyable and is not an array type

Can be initialized to an empty state as if by using
std::variant<std::monostate, First, Rest...>
*/
template <typename First, typename... Rest>
struct tagged_union {

    // Requirements: The return type of Visitor is default-constructible (or void)
    // Returns a value-initialized object when in an empty or invalid state
    template<typename Visitor>
    inline constexpr auto visit(Visitor vis) {
        return visit_impl<Visitor, First, Rest...>(vis);
    }

    template<typename Visitor>
    inline constexpr auto visit(Visitor vis) const {
        return visit_impl<Visitor, First, Rest...>(vis);
    }

    template<typename T>
    static constexpr int id = base1_index_of<T, First, Rest...>;

    int tag = 0;

    struct storage_t {
        alignas(max_align_of<First, Rest...>) char bytes[max_size_of<First, Rest...>];

        template <typename T>
        inline constexpr T& as() {
            static_assert(id<T> != 0, "tagged_union can not hold T");
            return reinterpret_cast<T&>(bytes);
        }

        template <typename T>
        inline constexpr const T& as() const {
            static_assert(id<T> != 0, "tagged_union can not hold T");
            return reinterpret_cast<const T&>(bytes);
        }

    } storage;

    constexpr tagged_union() noexcept = default;

    template<typename T>
    inline constexpr tagged_union(const T& val) noexcept {
        tag = id<T>;
        storage.template as<T>() = val;
    }

    template<typename T>
    inline constexpr tagged_union& operator=(const T& val) noexcept {
        tag = id<T>;
        storage.template as<T>() = val;
        return *this;
    }

private:
    template<typename Visitor, typename T1, typename... TRest>
    inline constexpr auto visit_impl(Visitor vis) const {
        if (tag == id<T1>) {
            return vis(storage.template as<T1>());
        }
        if constexpr (sizeof...(TRest) > 0) {
            return visit_impl<Visitor, TRest...>(vis);
        }
        else {
            using ReturnType = decltype(vis(declval<First&>()));
            if constexpr (!type_traits::is_void_v<ReturnType>) return ReturnType{};
        }
    }

    template<typename Visitor, typename T1, typename... TRest>
    inline constexpr auto visit_impl(Visitor vis) {
        if (tag == id<T1>) {
            return vis(storage.template as<T1>());
        }
        if constexpr (sizeof...(TRest) > 0) {
            return visit_impl<Visitor, TRest...>(vis);
        }
        else {
            using ReturnType = decltype(vis(declval<First&>()));
            if constexpr (!type_traits::is_void_v<ReturnType>) return ReturnType{};
        }
    }
};

template<class... Ts> struct overloaded : Ts... { using Ts::operator()...; };
template<class... Ts> overloaded(Ts...)->overloaded<Ts...>;
