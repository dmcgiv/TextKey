TextKey
=======

Simple wrapper for key/value strings to allow processing of values before returning

Adds ability to have key/value insert within key values.

Insert key format : $(key)

e.g
product_name = "Washing Machine"

product_price = "$1200"

product_details = "$(product_name) costs $(product_cost)"


product_details when processed with result in "Washing Machine costs $1200"
