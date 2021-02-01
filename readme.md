# HowToBeAHelper
Diese Software ist ein Hilfstool für das Pen and Paper How To Be A Hero. Um mehr über das Pen and Paper System zu erfahren, klicke auf den folgenden Link:
https://howtobeahero.de/index.php?title=Hauptseite


## Module
Seit der 1.0.10.0 besitzt How to be a Helper eine Modul-API. Damit können Modulersteller das allgemeine Verhalten von Charakteren ändern, sowie ihre Regelwerke direkt in das Programm integrieren. Um Module zu aktivieren, einfach die Datei des gewünschten Moduls (*.htbam) in den *modules*-Ordner im Hauptverzeichnis (einfach durch HTBAH mit dem + öffnen) gezogen werden und das Programm neugestartet werden.

### Eigenes Modul erstellen
Direkt vorweg: Das Erstellen von Modulen funktioniert über insgesamt 2 Dateien. Eine Datei, die Modul-Meta (\*.xml) sowie eine weitere optionale Datei, welche das Regelwerk darstellt (*.md, Markdown möglich). 
```xml
<?xml version="1.0" encoding="UTF-8"?>
<module>
    <meta>
        <name>Detectives in Dublin</name>
        <description>Detectives in Dublin ist ein modulares Detektiv-Regelwerk welches, wie der Name bereits vermuten lässt, in Dublin spielt.</description>
        <version>1.0</version>
        <author>DasDarki</author>
        <icon>https://www.dentons.com/-/media/images/website/background-images/offices/dublin/dublin-2.ashx?h=375&amp;la=de-DE&amp;w=475&amp;crop=1</icon>
    </meta>
    <skills>
        <acting></acting>
        <knowledge>
            <skill>
                Magiekunde
            </skill>
        </knowledge>
        <social></social>
    </skills>
    <form>
        <row>
            <column>
                <input type="number" label="Regisbindung" placeholder="Regiswert eingeben" key="regis" dblaction="dice_roll"/>
            </column>
        </row>
    </form>
</module>
```
Dies ist eine Beispiel Modul-Meta. Ganz oben stehen wichtige Informationen: Name, Beschreibung, Version, Author und optional ein Link zu einem Icon. Bitte beachtet beim Icon, dass wenn der Link **&**-Zeichen hat, müssen die durch **\&amp;** ersetzt werden.
Danach kommt der Skills-Reiter. Dort können, wie bei der Magiekunde, eigene Fertigkeiten hinzugefügt werden. Diese werden dann automatisch in die Vorschlags-Funktion von HTBAH integriert.   
Zuletzt könnt ihr den Charakter-Viewer und Editor mit dem **form**-Feld anpassen. Hierbei gilt das Row-Column-Layout. Bei den Inputs gibt es bislang nur die Typen **number** (Ganzzahl) und **text**. Der Schlüssel (key) muss angegeben werden und muss einzigartig sein. Bei der **dblaction** kann man bislang nur *dice_roll* eintragen, wodurch der Proben-Checker im Tool für dieses Feld aktiviert wird.

Nachdem beide Datein oder zumindest die Meta angelegt wurde, müsst ihr einfach nur die beiden Dateien nehmen und auf den HowToBeAHelper.Packer(.exe) ziehen, welcher daraus ein fähiges Modul (*.htbam) generiert. Das Packer-Tool findet ihr im Hauptverzeichnis.