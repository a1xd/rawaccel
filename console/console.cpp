#include <iostream>

#include <rawaccel-io.hpp>

#include "parse.hpp"

namespace ra = rawaccel;

int main(int argc, char** argv) {
	try {
		ra::write(ra::parse(argc, argv));
	}
	catch (const std::system_error& e) {
		std::cerr << e.what() << " (" << e.code() << ")\n";
	}
	catch (const std::exception& e) {
		std::cerr << e.what() << '\n';
	}
}
