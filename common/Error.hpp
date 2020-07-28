#pragma once

#include <iostream>


namespace rawaccel {
	void error(const char* s) { 
		throw std::domain_error(s); 
	}
}
