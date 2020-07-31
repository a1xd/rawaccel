#pragma once

#include <stdexcept>

namespace rawaccel {

	void error(const char* s) { 
		throw std::domain_error(s); 
	}

}
