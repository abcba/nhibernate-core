using System;
									  "INSERT INTO parent_history SELECT nextval('parent_history_histid_seq'), now(), NEW.*;" +
				command.CommandText = "DELETE from parent_history;";